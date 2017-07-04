namespace Microsoft.Telco.DigitalAssistant.Logging.Model
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class ConversationIdContainer : TableEntity
    {        
        public ConversationIdContainer()
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = string.Empty;
        }

        public string ContainerName { get; set; }

        public string ConversationId { get; set; }
    }
}
