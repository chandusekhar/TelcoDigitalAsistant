/// <summary>
/// Luis Guerrero Guirado (luguerre@microsoft.com)
/// </summary>
namespace Microsoft.Telco.DigitalAssistant.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Logging.Model;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// This class represent the logging mecanishm for the Bot Framework 
    /// </summary>
    public class LoggingManager
    {
        /// <summary>
        /// Private Azure Storage Connection string
        /// </summary>
        private static string storageConnection;

        /// <summary>
        /// Name of the Azure Table where messages will be stored
        /// </summary>
        private static string tableName;

        /// <summary>
        /// List of Activities keep in memory
        /// </summary>
        private static List<IMessageActivity> responses = new List<IMessageActivity>();
        private TelemetryClient client = new TelemetryClient();

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable the logging system
        /// </summary>
        public static bool DisableLogging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable if all message will be keep in memory
        /// </summary>
        public static bool KeepResponseInMemory { get; set; }

        /// <summary>
        /// Initialize the logging system
        /// </summary>
        public static void Initialize()
        {
            storageConnection = ConfigurationManager.AppSettings["LoggingStorageAccount"];
            tableName = ConfigurationManager.AppSettings["LoggingTableName"];
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Clear the Activities keep in memory
        /// </summary>
        public static void ResetResponses()
        {
            responses.Clear();
        }

        /// <summary>
        /// Gets the list of Activies keep in memory
        /// </summary>
        /// <returns>The list of Message Activities</returns>
        public static List<IMessageActivity> GetResponses()
        {
            return responses.ToList();
        }

        /// <summary>
        /// Log the message to Azure Storage
        /// </summary>
        /// <param name="value">A reference to the Activity</param>
        public void LogMessageActivity(IMessageActivity value)
        {
            if (!DisableLogging)
            {
                if (!DisableLogging)
                {
                    Task.Factory.StartNew(
                        (arg) =>
                        {
                            InsertConversationItem((IActivity)arg);
                        },
                        value,
                        TaskCreationOptions.HideScheduler | TaskCreationOptions.PreferFairness);
                }
            }

            if (KeepResponseInMemory)
            {
                responses.Add(value);
            }
        }

        /// <summary>
        /// Log the message to Azure Storage
        /// </summary>
        /// <param name="value">A reference to the Activity</param>
        public void LogConversation(IActivity value)
        {
            if (!DisableLogging)
            {
                Task.Factory.StartNew(
                    (arg) =>
                    {
                        InsertConversationItem((IActivity)arg);
                    },
                    value,
                    TaskCreationOptions.HideScheduler | TaskCreationOptions.PreferFairness);
            }
        }

        /// <summary>
        /// Gets all the messages for a given conversation id
        /// </summary>
        /// <param name="conversationId">The id for the conversation you want to get all messages</param>
        /// <returns>A list for all the messages</returns>
        public List<ConversationItem> GetConversation(string conversationId)
        {
            List<ConversationItem> result = new List<Model.ConversationItem>();

            if (!string.IsNullOrEmpty(conversationId))
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
                var tableClient = account.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);

                TableQuery<ConversationItem> query = new TableQuery<ConversationItem>();
                query = query.Where(TableQuery.GenerateFilterCondition("ConversationId", "eq", conversationId));

                result.AddRange(table.ExecuteQuery(query));
            }

            return result;
        }

        /// <summary>
        /// Read the content of a blob in a container
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="blobName">Name of the blob</param>
        /// <returns>The string with the content of the blob</returns>
        internal string GetBlobContent(string containerName, string blobName)
        {
            string result = null;

            if (!string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(blobName))
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                var blob = container.GetBlockBlobReference(blobName);
                result = blob.DownloadText();
            }

            return result;
        }

        /// <summary>
        /// Insert the message into the Azure Storage Account
        /// </summary>
        /// <param name="value">Activity to be saved</param>
        private void InsertConversationItem(IActivity value)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented);
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            ConversationItem item = new ConversationItem(value.Conversation.Id);

            TableOperation inserOperation = TableOperation.Insert(item);
            table.Execute(inserOperation);

            client.TrackEvent("InsertConversationItem");

            string containerName = GetContainerFromConversationId(value.Conversation.Id);
            if (!string.IsNullOrEmpty(containerName))
            {
                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                container.CreateIfNotExists();
                var blob = container.GetBlockBlobReference(item.BlobName);
                blob.UploadText(json);
            }
        }

        /// <summary>
        /// Gets the name of the container based on the conversation id
        /// </summary>
        /// <param name="value">Conversation id</param>
        /// <returns>The name of the container where the conversation is</returns>
        private string GetContainerFromConversationId(string value)
        {
            string result = null;

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference("converstaioncontainer");
            table.CreateIfNotExists();

            var containerItem = table.CreateQuery<ConversationIdContainer>()
                .Where(p => p.ConversationId == value)
                .FirstOrDefault();

            if (containerItem == null)
            {
                containerItem = new ConversationIdContainer()
                {
                    ContainerName = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant(),
                    ConversationId = value
                };

                TableOperation inserOperation = TableOperation.Insert(containerItem);
                table.Execute(inserOperation);

                result = containerItem.ContainerName;
            }
            else
            {
                result = containerItem.ContainerName;
            }

            return result;
        }
    }
}
