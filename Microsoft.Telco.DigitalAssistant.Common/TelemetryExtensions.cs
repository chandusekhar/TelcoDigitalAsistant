namespace Microsoft.Telco.DigitalAssistant.Common
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Bot.Builder.Dialogs;
    using Newtonsoft.Json;

    /// <summary>
    /// This class have the Telemetry Extension to send information to Application Insights
    /// </summary>
    public static class TelemetryExtensions
    {
        /// <summary>
        /// Create a trace telemetry item
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="message">Message to send</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>A <see cref="TraceTelemetry"/> item</returns>
        public static TraceTelemetry CreateTraceTelemetry(this IDialogContext context, string message = null, IDictionary<string, string> properties = null)
        {
            TraceTelemetry result = new TraceTelemetry(message);
            result.Properties.Add("ConversationData", JsonConvert.SerializeObject(context.ConversationData));
            result.Properties.Add("PrivateConversationData", JsonConvert.SerializeObject(context.PrivateConversationData));
            result.Properties.Add("UserData", JsonConvert.SerializeObject(context.UserData));

            var replymessage = context.MakeMessage();
            result.Properties.Add("ConversationId", replymessage.Conversation.Id);
            result.Properties.Add("UserId", replymessage.Recipient.Id);

            if (properties != null)
            {
                foreach (var p in properties)
                {
                    result.Properties.Add(p);
                }
            }

            return result;
        }

        /// <summary>
        /// Create a event item
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="message">Message to send</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>A <see cref="EventTelemetry"/> item</returns>
        public static EventTelemetry CreateEventTelemetry(this IDialogContext context, string message = null, IDictionary<string, string> properties = null)
        {
            EventTelemetry result = new EventTelemetry(message);
            result.Properties.Add("ConversationData", JsonConvert.SerializeObject(context.ConversationData));
            result.Properties.Add("PrivateConversationData", JsonConvert.SerializeObject(context.PrivateConversationData));
            result.Properties.Add("UserData", JsonConvert.SerializeObject(context.UserData));

            var replyMessage = context.MakeMessage();
            result.Properties.Add("ConversationId", replyMessage.Conversation.Id);
            result.Properties.Add("UserId", replyMessage.Recipient.Id);

            if (properties != null)
            {
                foreach (var p in properties)
                {
                    result.Properties.Add(p);
                }
            }

            return result;
        }

        /// <summary>
        /// Create a metric element
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="metricValue">Name for the metric</param>
        /// <param name="value">Value for the metric</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>A <see cref="MetricTelemetry"/> item</returns>
        public static MetricTelemetry CreateMetricTelemetry(this IDialogContext context, string metricValue = null, double value = 0d, IDictionary<string, string> properties = null)
        {
            MetricTelemetry result = new MetricTelemetry(metricValue, value);
            result.Properties.Add("ConversationData", JsonConvert.SerializeObject(context.ConversationData));
            result.Properties.Add("PrivateConversationData", JsonConvert.SerializeObject(context.PrivateConversationData));
            result.Properties.Add("UserData", JsonConvert.SerializeObject(context.UserData));

            var replyMessage = context.MakeMessage();
            result.Properties.Add("ConversationId", replyMessage.Conversation.Id);
            result.Properties.Add("UserId", replyMessage.Recipient.Id);

            if (properties != null)
            {
                foreach (var p in properties)
                {
                    result.Properties.Add(p);
                }
            }

            return result;
        }

        /// <summary>
        /// Create an exception item
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="exception">Exception value</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>A <see cref="ExceptionTelemetry"/> item</returns>
        public static ExceptionTelemetry CreateExceptionTelemetry(this IDialogContext context, System.Exception exception, IDictionary<string, string> properties = null)
        {
            ExceptionTelemetry result = new ExceptionTelemetry(exception);
            result.Properties.Add("ConversationData", JsonConvert.SerializeObject(context.ConversationData));
            result.Properties.Add("PrivateConversationData", JsonConvert.SerializeObject(context.PrivateConversationData));
            result.Properties.Add("UserData", JsonConvert.SerializeObject(context.UserData));

            var replyMessage = context.MakeMessage();
            result.Properties.Add("ConversationId", replyMessage.Conversation.Id);
            result.Properties.Add("UserId", replyMessage.Recipient.Id);

            if (properties != null)
            {
                foreach (var p in properties)
                {
                    result.Properties.Add(p);
                }
            }

            return result;
        }
    }
}
