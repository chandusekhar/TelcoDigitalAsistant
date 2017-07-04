namespace Microsoft.Telco.DigitalAssistant.Data.Model
{
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;

    /// <summary>
    /// Base class for all the entities
    /// </summary>
    public class Entity : Document
    {
        /// <summary>
        /// Gets or sets the partition id
        /// </summary>
        [JsonProperty(PropertyName = "partitionid")]
        public string PartitionId { get; set; }
    }
}
