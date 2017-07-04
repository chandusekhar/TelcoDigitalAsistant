namespace Microsoft.Telco.DigitalAssistant.Data.Model
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represent a Dialog Metric
    /// </summary>
    public class DialogMetric : Entity
    {
        /// <summary>
        /// Gets or sets when the dialogs start
        /// </summary>
        [JsonProperty(PropertyName = "start")]
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets when the dialogs ends
        /// </summary>
        [JsonProperty(PropertyName = "end")]
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the conversation id of the dialog
        /// </summary>
        [JsonProperty(PropertyName = "conversationid")]
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the dialog name
        /// </summary>
        [JsonProperty(PropertyName = "dialogname")]
        public string DialogName { get; set; }
    }
}
