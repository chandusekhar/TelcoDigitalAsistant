/// <summary>
/// Luis Guerrero Guirado (luguerre@microsoft.com)
/// </summary>
namespace Microsoft.Telco.DigitalAssistant.Logging
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    public static class BotConnectorExtensions
    {
        /// <summary>
        /// Reply a message to the user
        /// </summary>
        /// <param name="context">Current Dialog context</param>
        /// <param name="message">Message to send</param>
        /// <returns>Task to monitor the progress</returns>
        public static async Task ReplyMessage(this IDialogContext context, string message)
        {
            var reply = context.MakeMessage();
            reply.Text = message;
            new LoggingManager().LogMessageActivity(reply);
            await context.PostAsync(reply);
        }

        /// <summary>
        /// Reply a message to the user
        /// </summary>
        /// <param name="context">Current Dialog context</param>
        /// <param name="message">Message to send</param>
        /// <returns>Task to monitor the progress</returns>
        public static async Task ReplyMessage(this IDialogContext context, IMessageActivity message)
        {
            new LoggingManager().LogMessageActivity(message);
            await context.PostAsync(message);
        }
    }
}
