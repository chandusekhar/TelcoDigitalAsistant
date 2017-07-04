namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Common;
    using Microsoft.Telco.DigitalAssistant.Data;
    using Microsoft.Telco.DigitalAssistant.Data.Model;
    using Microsoft.Telco.DigitalAssistant.Logging;

    /// <summary>
    /// This is the base dialog class
    /// </summary>
    /// <typeparam name="Type">The type of the DocumentDb entity</typeparam>
    [Serializable]
    public class BaseDataDialog<Type> : LuisDialog<Type>
        where Type : Azure.Documents.Document
    {
        /// <summary>
        /// Private debug key
        /// </summary>
        private const string DebugKey = "__Debug";

        /// <summary>
        /// Private DialogId key
        /// </summary>
        private const string DialogIdKey = "__DialogIdKey";

        /// <summary>
        /// Private random class
        /// </summary>
        [NonSerialized]
        private static Random r = new Random();

        /// <summary>
        /// Gets or sets if debug is enabled on the Dialog
        /// </summary>
        private bool isDebugEnabled = false;

        /// <summary>
        /// Private reference to the <see cref="DialogMetric"/>
        /// </summary>
        [NonSerialized]
        private DialogMetric dialogMetric;

        /// <summary>
        /// Private Telemetry Client
        /// </summary>
        [NonSerialized]
        private TelemetryClient telemetry = new TelemetryClient();

        /// <summary>
        /// Private DocumentDb Repository
        /// </summary>
        [NonSerialized]
        private DocumentDBRepository<Type> client = new DocumentDBRepository<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDataDialog{Type}"/> class.
        /// </summary>
        public BaseDataDialog()
        {
#if DEBUG
            isDebugEnabled = true;
#endif
        }

        /// <summary>
        /// Gets the <see cref="TelemetryClient"/> reference
        /// </summary>
        protected TelemetryClient TelemetryClient
        {
            get
            {
                if (telemetry == null)
                {
                    telemetry = new TelemetryClient();
                }

                return telemetry;
            }
        }

        /// <summary>
        /// Gets a value indicating whether gets if the debuggin is enabled
        /// </summary>
        protected bool IsDebugEnabled
        {
            get
            {
                return isDebugEnabled;
            }
        }

        /// <summary>
        /// Gets the DataClient reference
        /// </summary>
        protected DocumentDBRepository<Type> DataClient
        {
            get
            {
                return client;
            }
        }

        /// <summary>
        /// Start the dialog
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <returns>A task to monitor the progress</returns>
        public override async Task StartAsync(IDialogContext context)
        {
            await base.StartAsync(context);

            // Send the StartDialog telemtry to know how many times a dialog is started
            TelemetryClient.TrackMetric(context.CreateMetricTelemetry("StartDialog", 1));

            if (context.PrivateConversationData.ContainsKey(DebugKey))
            {
                isDebugEnabled = context.PrivateConversationData.GetValue<bool>(DebugKey);
            }
            else
            {
                context.PrivateConversationData.SetValue(DebugKey, isDebugEnabled);
            }

            // gets the dialog based on the name of the instance class
            string dialogKey = string.Concat(DialogIdKey, GetType().Name);
            if (context.PrivateConversationData.ContainsKey(dialogKey))
            {
                // inside the PrivateConverstaionData is  DialogMetric so getting the value from there
                string dialogTraceId = context.PrivateConversationData.GetValue<string>(dialogKey);
                dialogMetric = await new DialogMetricManager().GetDialogMetricFromId(dialogTraceId);
            }
            else
            {
                // create a new Dialog metric and set the Start as DateTiem.UtcNow
                dialogMetric = new DialogMetric()
                {
                    Start = DateTime.UtcNow,
                    ConversationId = context.Activity.Conversation.Id,
                    PartitionId = context.Activity.ChannelId,
                    DialogName = GetType().Name
                };

                dialogMetric = await new DialogMetricManager().CreateDialogMetric(dialogMetric);
                context.PrivateConversationData.SetValue(dialogKey, dialogMetric.Id);
            }
        }

        /// <summary>
        /// Ends the dialog
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <returns>A task to monitor the progress</returns>
        public virtual async Task EndAsync(IDialogContext context)
        {
            string dialogKey = string.Concat(DialogIdKey, GetType().Name);
            if (context.PrivateConversationData.ContainsKey(dialogKey))
            {
                using (DialogMetricManager manager = new DialogMetricManager())
                {
                    string dialogTraceId = context.PrivateConversationData.GetValue<string>(dialogKey);
                    dialogMetric = await manager.GetDialogMetricFromId(dialogTraceId);
                    dialogMetric.End = DateTime.UtcNow;
                    await manager.UpdateDialogMetric(dialogMetric);
                }

                context.PrivateConversationData.RemoveValue(dialogKey);
            }
        }

        /// <summary>
        /// Log reply
        /// </summary>
        /// <param name="value">A reference to the <see cref="IMessageActivity"/> value</param>
        public void LogReply(IMessageActivity value)
        {
            new LoggingManager().LogMessageActivity(value);
        }

        /// <summary>
        /// Gets a Random integer value
        /// </summary>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>The integer value</returns>
        protected int GetRandomInt(int minValue, int maxValue)
        {
            return r.Next(minValue, maxValue);
        }

        /// <summary>
        /// Gets the best intent from a <see cref="LuisResult"/>
        /// </summary>
        /// <param name="value">The Luis result</param>
        /// <returns>A referene to the <see cref="IntentRecommendation"/></returns>
        protected override IntentRecommendation BestIntentFrom(LuisResult value)
        {
            IntentRecommendation result = base.BestIntentFrom(value);

            // if the intent has a score lower of 50% (0.5d) value, from my perspetive is a None intents
            if (result != null && result.Score < 0.5d)
            {
                result = new IntentRecommendation("None", 1d);
            }
            else if (result == null)
            {
                result = new IntentRecommendation(Names.InternalMessageKey, 1d);
            }

            return result;
        }

        /// <summary>
        /// Gets the Luis Query from the message
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="message">Message activity</param>
        /// <returns>The text that will be send to LUIS</returns>
        protected override async Task<string> GetLuisQueryTextAsync(IDialogContext context, IMessageActivity message)
        {
            var result = await base.GetLuisQueryTextAsync(context, message);

            // this method is for activate the debug option during the dialog
            if (result.ToLowerInvariant() == "debug")
            {
                context.PrivateConversationData.SetValue(DebugKey, isDebugEnabled);
                isDebugEnabled = !isDebugEnabled;
                await context.ReplyMessage($"Debug is now {isDebugEnabled}");
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Method called when a new message is received
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="item">Message activity</param>
        /// <returns>A Task to monitor the progress</returns>
        protected override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            // This line is core in how we track Telemetry from Application Insights perspetive.
            // Operation context id is how Application Insights correlate all the messages together,
            // so correlating them by the same conversation id is the best way to review what happen in a 
            // converstaion level
            TelemetryClient.Context.Operation.Id = context.Activity.Conversation.Id;
            TelemetryClient.Context.Operation.Name = GetType().Name;
            TelemetryClient.TrackEvent(context.CreateEventTelemetry("MessageReceived"));
            return base.MessageReceived(context, item);
        }

        /// <summary>
        /// Dispatch intent to class handler
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="item">Message item</param>
        /// <param name="bestInent">A reference to the best intent</param>
        /// <param name="result">LUIS result</param>
        /// <returns>A task to monitor the progress</returns>
        protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestInent, LuisResult result)
        {
            TelemetryClient.TrackEvent(context.CreateEventTelemetry(
                "DispatchToIntentHandler",
                new Dictionary<string, string>()
                {
                    { "Luis", bestInent.Intent },
                    { "Score", bestInent.Score.ToString() }
                }));

            TelemetryClient.TrackMetric(context.CreateMetricTelemetry("Message", 1));
            new LuisLoggingManager().LogLuisActivity(context, result);
            if (IsDebugEnabled)
            {
                context.PostAsync($"DEBUG: Luis Intent {bestInent.Intent} Score {bestInent.Score} in the Dialog {GetType().Name}");
            }

            return base.DispatchToIntentHandler(context, item, bestInent, result);
        }

        /// <summary>
        /// Gets the Users based on the current Activity
        /// </summary>
        /// <param name="value">A reference to the <see cref="IActivity"/> value</param>
        /// <returns>A reference to the user</returns>
        protected async Task<User> GetUser(IActivity value)
        {
            User result = null;

            if (value != null)
            {
                UserManager manager = new UserManager();
                result = await manager.GetUserById(value.From.Id);
            }

            return result;
        }

        /// <summary>
        /// Save the user in the DocumentDb
        /// </summary>
        /// <param name="value">A reference to the <see cref="IActivity"/> value</param>
        /// <param name="user">A reference to the <see cref="User"/> value</param>
        /// <returns>A reference to the user</returns>
        protected async Task SaveUser(Activity value, User user)
        {
            if (value != null)
            {
                UserManager manager = new UserManager();
                await manager.SaveUser(user);
            }
        }
    }
}