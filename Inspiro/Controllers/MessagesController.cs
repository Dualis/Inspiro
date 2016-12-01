using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using Inspiro.Responders;
using System.Collections.Generic;
using Inspiro.DataModels;
using Inspiro.Responders;

namespace Inspiro
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                string text = (activity.Text ?? string.Empty).ToLower();
                int length = text.Length;
                string replyStr = string.Empty;

                string name = String.Empty;
                string userId = activity.From.Id.ToString();
                bool loggedIn = false;

                try
                {
                    loggedIn = await authoriseUser(activity, connector, text, userId);
                }
                catch (MobileServiceInvalidOperationException e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR! : {e.Message}");
                }

                if (!loggedIn)
                {
                    var responseOut = Request.CreateResponse(HttpStatusCode.OK);
                    return responseOut;
                }

                Accounts userAccount;
                try
                {
                    userAccount = await getUserAccount(name);
                }
                catch (MobileServiceInvalidOperationException e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR! : {e.Message}");
                    userAccount = new Accounts();
                }

                
                string responderName = userData.GetProperty<string>("Responder");
                System.Diagnostics.Debug.WriteLine(userData.GetProperty<string>("Responder"));
                if (responderName == null)
                {
                    responderName = "boring";
                    userData.SetProperty<string>("Responder", responderName);
                }
                Responder responder = Responder.GetResponder(responderName);

                //If block for user input
                if (text.StartsWith("clear"))
                {
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    Auth auth = await getUserAuth(userId);
                    await AzureManager.AzureManagerInstance.DeleteAuths(auth);
                    replyStr = "User data cleared";
                }
                else if (text.StartsWith("use"))
                {
                    //Sets the users preferred responder
                    //Does not check if a responder exists with that name
                    userData.SetProperty<string>("Responder", text.Substring(4));
                    responderName = userData.GetProperty<string>("Responder");
                    responder = Responder.GetResponder(responderName);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    Activity greetingCard = await buildCardResponse(activity, $"Now using {responder.getName()}", "", responder.greeting(), responder.neutralImageUrl());
                    await connector.Conversations.SendToConversationAsync(greetingCard);
                }
                else if (text.StartsWith("quote"))
                {
                    string quote = getRandomQuote(responder.getName());
                    Activity quoteCard = await buildCardResponse(activity, "", "", quote, responder.neutralImageUrl());
                    await connector.Conversations.SendToConversationAsync(quoteCard);
                }
                else if (text.StartsWith("balance"))
                {
                    Activity balanceCardReply = await getFullBalance(userAccount, responder, activity);
                    await connector.Conversations.SendToConversationAsync(balanceCardReply);
                }
                else if (text.StartsWith("cheque"))
                {
                    
                    try
                    {
                        string change = text.Substring(7);
                        double changeValue = Double.Parse(change);
                        userAccount.Cheque = userAccount.Cheque + changeValue;

                        await AzureManager.AzureManagerInstance.UpdateAccounts(userAccount);
                        string imageUrl;
                        if (changeValue >= 0.0) imageUrl = responder.positiveImageUrl();
                        else imageUrl = responder.negativeImageUrl();

                        Activity confirmCard = await buildCardResponse(activity, responder.affirmative(), "Balance updated", $"{userAccount.Cheque}", imageUrl);
                        await connector.Conversations.SendToConversationAsync(confirmCard);

                    }
                    catch (Exception e)
                    {
                        Activity unkownCard = await buildCardResponse(activity, "", "", responder.unknown(), responder.negativeImageUrl());
                        await connector.Conversations.SendToConversationAsync(unkownCard);
                    }
                }
                else if (text.StartsWith("savings"))
                {
                    string change = text.Substring(8);
                    try
                    {
                        
                        
                        double changeValue = Double.Parse(change);
                        userAccount.Savings = userAccount.Savings + changeValue;

                        await AzureManager.AzureManagerInstance.UpdateAccounts(userAccount);
                        string imageUrl;
                        if (changeValue >= 0.0) imageUrl = responder.positiveImageUrl();
                        else imageUrl = responder.negativeImageUrl();

                        Activity confirmCard = await buildCardResponse(activity, responder.affirmative(), "Balance updated", $"{userAccount.Savings}", imageUrl);
                        await connector.Conversations.SendToConversationAsync(confirmCard);

                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR! : {e.Message}");
                        System.Diagnostics.Debug.WriteLine($"ERROR! : {change}");
                        Activity unkownCard = await buildCardResponse(activity, "", "", responder.unknown(), responder.negativeImageUrl());
                        await connector.Conversations.SendToConversationAsync(unkownCard);
                    }
                }
                else
                {
                    Activity unkownCard = await buildCardResponse(activity, "", "", responder.unknown(), responder.negativeImageUrl());
                    await connector.Conversations.SendToConversationAsync(unkownCard);
                }


                if (replyStr.Length > 0)
                {
                    // return our reply to the user
                    Activity reply = activity.CreateReply(replyStr);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> getFullBalance(Accounts userAccount, Responder responder, Activity activity)
        {
            string comment;
            string imageUrl;
            if (userAccount.Cheque + userAccount.Savings > 10000)
            {
                comment = responder.positive();
                imageUrl = responder.positiveImageUrl();
            }
            else if (userAccount.Cheque + userAccount.Savings > 100)
            {
                comment = responder.neutral();
                imageUrl = responder.neutralImageUrl();
            }
            else
            {
                comment = responder.negative();
                imageUrl = responder.negativeImageUrl();
            }

            Activity replyToConversation = activity.CreateReply(comment);
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();

            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: imageUrl));

            ThumbnailCard balanceCard = new ThumbnailCard()
            {
                Title = "Your Balance",
                Subtitle = $"Cheque Account: {userAccount.Cheque}\n Savings Account: {userAccount.Savings}",
                Images = cardImages
            };

            Attachment balanceAttachment = balanceCard.ToAttachment();
            replyToConversation.Attachments.Add(balanceAttachment);
            return replyToConversation;
        }

        private async Task<Activity> buildCardResponse(Activity activity, string comment, string title, string subtitle, string imageUrl)
        {
            Activity replyToConversation = activity.CreateReply(comment);
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();

            Attachment card = await buildCard(title, subtitle, imageUrl);
            replyToConversation.Attachments.Add(card);
            return replyToConversation;
        }

        private async Task<Attachment> buildCard(string title, string subtitle, string imageUrl)
        {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: imageUrl));

            ThumbnailCard card = new ThumbnailCard()
            {
                Title = title,
                Subtitle = subtitle,
                Images = cardImages
            };

            Attachment attachment = card.ToAttachment();
            return attachment;
        }

        private static async Task<Accounts> getUserAccount(string name)
        {
            List<Accounts> accounts = await AzureManager.AzureManagerInstance.GetAccounts();
            Accounts userAccounts = null;

            //Finds the account for this user
            foreach (Accounts acc in accounts)
            {
                if (acc.Owner.Equals(name))
                {
                    userAccounts = acc;
                    break;
                }
            }

            //Create new account
            if (userAccounts == null)
            {
                userAccounts = new Accounts()
                {
                    Owner = name,
                    Cheque = 0.0,
                    Savings = 0.0,
                    DateCreated = DateTime.Now,
                    DateAccessed = DateTime.Now

                };
                await AzureManager.AzureManagerInstance.AddAccounts(userAccounts);
            }

            return userAccounts;
        }

        private async Task<bool> authoriseUser(Activity activity, ConnectorClient connector, string text, string userId)
        {
            bool loggedIn = false;
            Auth userAuth = await getUserAuth(userId);
            //Create new auth
            if (userAuth == null)
            {
                userAuth = new Auth()
                {
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    DateAccessed = DateTime.Now

                };
                await AzureManager.AzureManagerInstance.AddAuth(userAuth);
                Activity reply = activity.CreateReply("Please set a name and password in the form \"set NAME:PASSWORD\"");
                await connector.Conversations.ReplyToActivityAsync(reply);
                loggedIn = true;
            }
            else if (DateTime.Now.Subtract(userAuth.DateAccessed).TotalHours > 24)
            {
                if (text.StartsWith("login"))
                {
                    if (text.Substring(6).Equals(userAuth.Password))
                    {
                        Activity reply = activity.CreateReply("Wrong password");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else loggedIn = true;
                }

            }
            else loggedIn = true;
            return loggedIn;
        }

        public async Task<Auth> getUserAuth(string userId)
        {
            List<Auth> auths = await AzureManager.AzureManagerInstance.GetAuths();
            Auth userAuth = null;
            

            //Finds the account for this user
            foreach (Auth auth in auths)
            {
                if (auth.UserId.Equals(userId))
                {
                    userAuth = auth;
                    break;
                }
            }

            return userAuth;
        }

        /// <summary>
        /// Polls an external api for a random quote from the author with the given name
        /// 
        /// </summary>
        private string getRandomQuote(string name)
        {
            //Sets name to anon if empty
            if (name == string.Empty) name = "Anonymous";
            string uriString = "http://en.wikiquote.org/w/api.php?action=query&prop=revisions&rvprop=content&format=php&titles=" + name;
            Uri address = new Uri(uriString);
            HttpWebRequest req = HttpWebRequest.Create(address) as HttpWebRequest;

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string response = reader.ReadToEnd();

            //Cleanup response
            //response = response.Substring(response.IndexOf("== Quotes =="));
            //response = response.Substring(response.IndexOf("==External links=="));


            string[] quotes = response.Split("*".ToCharArray());

            //TODO:More cleanup
            double index = 1.0 + new Random().NextDouble() * quotes.Length;
            return quotes[(int) index];
        }



        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}