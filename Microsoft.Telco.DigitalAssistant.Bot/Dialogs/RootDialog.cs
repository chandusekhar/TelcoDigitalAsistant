namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Common;
    using Microsoft.Telco.DigitalAssistant.Data.Model;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// Represent the Root Dialog
    /// </summary>
    [LuisModel("78011017-ba0a-457b-a32f-7b99286c1a6f", "")]
    [Serializable]
    public class RootDialog : BaseDataDialog<Data.Model.Entity>
    {
        /// <summary>
        /// Handle Hello intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        [LuisIntent("Hello")]
        public void Hello(IDialogContext context, LuisResult result)
        {
            context.Call(new HelloDialog(), HelloMessageReceived);
        }

        /// <summary>
        /// Handle LostPhone intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("LostPhone")]
        public async Task LostPhoneStart(IDialogContext context, LuisResult result)
        {
            await context.Forward(new LostPhoneDialog(), MessageReceivedInternal, context.Activity, CancellationToken.None);
        }

        /// <summary>
        /// Handle Help intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("Help")]
        public async Task HelpStart(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();
            HeroCard card = new HeroCard("Things I can help you", "Help");
            card.Buttons = new List<CardAction>();
            card.Buttons.Add(new CardAction("imBack", "I lost my device", null, "I lost my phone"));
            card.Buttons.Add(new CardAction("imBack", "Weather", null, "What is the weather today in Madrid?"));
            reply.Attachments.Add(card.ToAttachment());
            await context.ReplyMessage(reply);
        }

        /// <summary>
        /// Handle None intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new CortanaDialog(), MessageReceivedInternal, context.Activity, CancellationToken.None);
        }

        /// <summary>
        /// Handle InternalMessage intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent(Names.InternalMessageKey)]
        public Task InternalMessage(IDialogContext context, LuisResult result)
        {
            return Task.FromResult<object>(null);
        }

        private async Task HelloMessageReceived(IDialogContext context, IAwaitable<User> result)
        {
            await context.ReplyMessage("How I can help you? If you need more info say help.");
        }

        private async Task MessageReceivedInternal(IDialogContext context, IAwaitable<object> result)
        {
            await context.ReplyMessage("How I can help you? If you need more info say help.");
        }
    }
}