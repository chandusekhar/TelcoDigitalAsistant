namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Data.Model;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// Represent the Hello Dialog
    /// </summary>
    [LuisModel("78011017-ba0a-457b-a32f-7b99286c1a6f", "")]
    [Serializable]
    public class HelloDialog : BaseDataDialog<User>
    {
        /// <summary>
        /// A set of default hello strings
        /// </summary>
        private string[] helloStrings = new string[]
        {
            "Hey {0}, it´s nice to have you back",
            "Hello {0}",
            "Welcome {0}",
            "Hi {0}"
        };

        /// <summary>
        /// The method executed when the dialog start
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <returns>A task to monitor the progress</returns>
        public override async Task StartAsync(IDialogContext context)
        {
            await base.StartAsync(context);

            // Lookup for the user on the database
            var user = await GetUser(context.Activity);

            // if the user is already on the system, just say hello, otherwise ask for the name
            if (user != null && !string.IsNullOrEmpty(user.Nickname))
            {
                await context.ReplyMessage(string.Format(helloStrings[GetRandomInt(0, helloStrings.Length)], user.Nickname));
                context.Done(user);
            }
            else
            {
                await context.ReplyMessage("Hi, what´s your name?");
                context.Wait(GetName);
            }
        }

        /// <summary>
        /// Method that will execute when the user said their name
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">Message result</param>
        /// <returns>A task to monitor the progress</returns>
        private async Task GetName(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var user = await GetUser(activity);
            user.Nickname = activity.Text;
            await SaveUser(activity, user);
            await context.ReplyMessage($"Hi {user.Nickname}, welcome");
            context.Done(user);
            await EndAsync(context);
        }
    }
}