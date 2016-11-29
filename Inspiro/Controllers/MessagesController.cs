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
                
                string text = (activity.Text ?? string.Empty);
                int length = text.Length;
                string replyStr = string.Empty;

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