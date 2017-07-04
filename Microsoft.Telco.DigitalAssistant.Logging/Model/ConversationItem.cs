namespace Microsoft.Telco.DigitalAssistant.Logging.Model
{
    using Microsoft.Bot.Connector;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    using System;
    using Microsoft.Telco.DigitalAssistant.Common;

    public class ConversationItem : TableEntity
    {
        public ConversationItem() { }

        public ConversationItem(string converstaionId)
        {
            converstaionId.EnsureIsNotNullOrEmpty(nameof(converstaionId), " can´t be null");

            PartitionKey = converstaionId;
            ConversationId = converstaionId;
            Created = DateTime.UtcNow;
            BlobName = Guid.NewGuid().ToString();
            RowKey = string.Concat(converstaionId, "-", Guid.NewGuid());
        }

        public string ConversationId { get; set; }

        public DateTime Created { get; set; }

        public string BlobName { get; set; }

        public Activity ToActivity()
        {
            Microsoft.Bot.Connector.
            Activity result = null;

            string json = new LoggingManager().GetBlobContent(ConversationId, BlobName);
            result = JsonConvert.DeserializeObject<Activity>(json);

            return result;
        }
    }
}
