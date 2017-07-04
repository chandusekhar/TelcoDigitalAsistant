namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Telco.DigitalAssistant.Data.Model;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// Represent the default Cortana dialog in LUIS
    /// </summary>
    [LuisModel("c413b2ef-382c-45bd-8ff0-f76d60e2a821", "")]
    [Serializable]
    public class CortanaDialog : BaseDataDialog<Entity>
    {
        /// <summary>
        /// Handle Weather intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("builtin.intent.weather.check_weather")]
        public async Task Weather(IDialogContext context, LuisResult result)
        {
            await context.ReplyMessage("Let me check the weather for you");
            context.Wait(MessageReceived);
        }

        /// <summary>
        /// Handle None intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        [LuisIntent("builtin.intent.none")]
        public void None(IDialogContext context, LuisResult result)
        {
            context.Done<object>(null);
        }
    }
}