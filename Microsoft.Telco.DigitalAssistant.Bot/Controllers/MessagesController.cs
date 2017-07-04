namespace Microsoft.Telco.DigitalAssistant.Bot
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Data;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// Represent the root message controler for the Bot Framework
    /// </summary>
#if !DEBUG
    [BotAuthentication]
#endif
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// <param name="activity">Activity value</param>
        /// <returns>The <see cref="HttpRequestMessage"/> reply</returns>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // Check for the user if exits a the very beguining of the conversation
            await new UserManager().CheckUser(activity.From.Id, activity.From.Name, activity.ChannelId);

            // Log the message to Azure Storage
            new LoggingManager().LogMessageActivity(activity);

            if (activity.Type == ActivityTypes.Message)
            {
                // Start the root conversation
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }

            if (Request != null)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Handle system messages coming from the Bot Connector
        /// </summary>
        /// <param name="message">Message value</param>
        /// <returns>The same activity</returns>
        private Activity HandleSystemMessage(Activity message)
        {
            WebApiApplication.Telemetry.TrackEvent(@"SystemMessage", new Dictionary<string, string> { { @"Type", message.Type } });
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