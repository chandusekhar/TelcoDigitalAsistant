namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// Represent the Lost Phone dialog
    /// </summary>
    [LuisModel("78011017-ba0a-457b-a32f-7b99286c1a6f", "")]
    [Serializable]
    public class LostPhoneDialog : BaseDataDialog<Data.Model.Entity>
    {
        /// <summary>
        /// When the device was lost
        /// </summary>
        private DateTime when;

        /// <summary>
        /// Handle LostPhone intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("LostPhone")]
        public async Task LostPhoneStart(IDialogContext context, LuisResult result)
        {
            await context.ReplyMessage("It´s seem that lost you phone, can you tell me when?");
            context.Wait(MessageReceived);
        }

        /// <summary>
        /// Handle LostPhoneWhen intent
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Luis result</param>
        /// <returns>A task to monitor the progress</returns>
        [LuisIntent("LostPhone.When")]
        public async Task LostPhoneWhen(IDialogContext context, LuisResult result)
        {
            EntityRecommendation datetimeEntity = null;
            if (result.TryFindEntity("builtin.datetime.date", out datetimeEntity))
            {
                DateTime value;

                // we try to parse a valid date, if not ask again
                if (DateTime.TryParse((string)datetimeEntity.Resolution.First().Value, out value))
                {
                    when = value;
                    await context.ReplyMessage("Ok, and where do you lost your phone?");
                    context.Wait(LostPhoneWhen);
                }
            }
            else
            {
                await context.ReplyMessage("Sorry I didn´t get that, I´m looking for a date");
                context.Wait(MessageReceived);
            }
        }

        /// <summary>
        /// Handle the last part of the dialog
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="item">Message from the bot connector</param>
        /// <returns>A task to monitor the progress</returns>
        public async Task LostPhoneWhen(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            await context.ReplyMessage("Fantastic, let me summarize that for you");
            var reply = context.MakeMessage();
            ReceiptCard card = new ReceiptCard();
            card.Items = new List<ReceiptItem>();
            card.Title = "Here is your summary";
            card.Items.Add(new ReceiptItem("When", when.ToLongDateString()));
            card.Items.Add(new ReceiptItem("Where", message.Text));
            reply.Attachments.Add(card.ToAttachment());
            await context.ReplyMessage(reply);
            context.Wait(MessageReceived);
            context.Done(string.Empty);
            await EndAsync(context);
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
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.ReplyMessage(message);
            context.Wait(MessageReceived);
        }
    }
}