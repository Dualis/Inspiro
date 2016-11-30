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
//using Inspiro.Responders;

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
                bool loggedIn = await authoriseUser(activity, connector, text, userId);

                if (!loggedIn)
                {
                    var responseOut = Request.CreateResponse(HttpStatusCode.OK);
                    return responseOut;
                }

                Accounts userAccount = await getUserAccount(name);


                //If block for user input
                if (text.StartsWith("clear"))
                {
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    replyStr = "User data cleared";
                }
                else if (text.StartsWith("use"))
                {
                    //Sets the users preferred responder
                    //Does not check if a responder exists with that name
                    userData.SetProperty<string>("Responder", text.Substring(4));

                }
                else if (text.StartsWith("quote"))
                {
                    string responderName = userData.GetProperty<string>("Responder");
                    Responder responder = Responder.GetResponder(responderName);
                    replyStr = getRandomQuote(responder.getName());
                }
                else if (text.ToLower().Contains("balance"))
                {

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

        private static async Task<bool> authoriseUser(Activity activity, ConnectorClient connector, string text, string userId)
        {
            List<Auth> auths = await AzureManager.AzureManagerInstance.GetAuths();
            Auth userAuth = null;
            bool loggedIn = false;

            //Finds the account for this user
            foreach (Auth auth in auths)
            {
                if (auth.UserId.Equals(userId))
                {
                    userAuth = auth;
                    break;
                }
            }

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
            response = response.Substring(response.IndexOf("== Quotes =="));
            response = response.Substring(response.IndexOf("==External links=="));


            string[] quotes = response.Split("\n* ".ToCharArray());

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