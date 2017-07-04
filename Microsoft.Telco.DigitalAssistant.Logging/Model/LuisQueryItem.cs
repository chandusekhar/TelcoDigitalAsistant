namespace Microsoft.Telco.DigitalAssistant.Logging.Model
{
   
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using Microsoft.Telco.DigitalAssistant.Common;

    public class LuisQueryItem : TableEntity
    {
        public LuisQueryItem(string converstaionId)
        {
            converstaionId.EnsureIsNotNullOrEmpty(nameof(converstaionId), " can´t be null");

            PartitionKey = converstaionId;
            ConversationId = converstaionId;
            Created = DateTime.UtcNow;
            BlobName = Guid.NewGuid().ToString();
            RowKey = string.Concat(converstaionId, "-", Guid.NewGuid());
        }

        public string ConversationId { get; set; }

        public string BlobName { get; set; }

        public DateTime Created { get; set; }

        public string OriginalRequest { get; set; }

        public string Entities { get; set; }

        public double Score { get; set; }

        public double NextItemScore { get; set; }

        public string NextItemItent { get; set; }

    }
}
