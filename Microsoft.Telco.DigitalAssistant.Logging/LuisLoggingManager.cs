/// <summary>
/// Luis Guerrero Guirado (luguerre@microsoft.com)
/// </summary>
namespace Microsoft.Telco.DigitalAssistant.Logging
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Telco.DigitalAssistant.Logging.Model;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// This class log all the response from LUIS (https://luis.ai)
    /// </summary>
    public class LuisLoggingManager
    {
        /// <summary>
        /// Gets the Azure Storage Connection string
        /// </summary>
        private static string storageConnection;

        /// <summary>
        /// Gets the Azure Table name
        /// </summary>
        private static string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuisLoggingManager"/> class.
        /// </summary>
        public LuisLoggingManager()
        {
            storageConnection = ConfigurationManager.AppSettings["LoggingStorageAccount"];
            tableName = ConfigurationManager.AppSettings["LuisLoggingTableName"];
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Log the LUIS result
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result">LUIS result</param>
        public void LogLuisActivity(IDialogContext context, LuisResult result)
        {
            LuisQueryItem item = new LuisQueryItem(Guid.NewGuid().ToString());
            item.OriginalRequest = result.Query;
            var intent = result.Intents.OrderByDescending(p => p.Score).FirstOrDefault();
            if (intent != null)
            {
                item.Entities = intent.Intent;
                item.Score = intent.Score.HasValue ? intent.Score.Value : -1d;
            }

            var next = result.Intents.OrderByDescending(p => p.Score).Skip(1).FirstOrDefault();
            if (next != null)
            {
                item.NextItemScore = next.Score.Value;
                item.NextItemItent = next.Intent;
            }

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnection);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            TableOperation insert = TableOperation.Insert(item);
            table.Execute(insert);
        }
    }
}
