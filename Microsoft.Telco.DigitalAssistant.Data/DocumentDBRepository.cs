namespace Microsoft.Telco.DigitalAssistant.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    /// <summary>
    /// Represent the Document Repository for a type
    /// </summary>
    /// <typeparam name="T">The type of the entity to work. This entity should inherit from Document</typeparam>
    public class DocumentDBRepository<T> : IDisposable
           where T : Document
    {
        /// <summary>
        /// Gets the database id
        /// </summary>
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];

        /// <summary>
        /// Gets the collection id
        /// </summary>
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];

        /// <summary>
        /// Private reference to the DocumentDbClient
        /// </summary>
        private DocumentClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDBRepository{T}"/> class.
        /// </summary>
        public DocumentDBRepository()
        {
            client = new DocumentClient(
                new Uri(
                    ConfigurationManager.AppSettings["endpoint"]),
                    ConfigurationManager.AppSettings["authKey"],
                    new ConnectionPolicy()
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    });
        }

        /// <summary>
        /// Gets a collection of items that match the predicate
        /// </summary>
        /// <param name="predicate">A lambda function to evaluate against the model</param>
        /// <returns>A collection of element of type T</returns>
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true
                })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        /// <summary>
        /// Update the elemnet
        /// </summary>
        /// <param name="id">Id for the item</param>
        /// <param name="item">A reference to the item</param>
        /// <returns>A task to monitor the progress</returns>
        public async Task<T> UpdateItemAsync(string id, T item)
        {
            return (dynamic)(await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item)).Resource;
        }

        /// <summary>
        /// Delete the element
        /// </summary>
        /// <param name="id">Id for the item</param>
        /// <param name="item">A reference to the item</param>
        /// <returns>A task to monitor the progress</returns>
        public async Task<T> DeleteItemAsync(string id, T item)
        {
            return (dynamic)(await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id))).Resource;
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        /// <param name="item">A reference to the items</param>
        /// <returns>A reference to the new item</returns>
        public async Task<T> CreateItemAsync(T item)
        {
            return (dynamic)(await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item)).Resource;
        }

        /// <summary>
        /// Dispose the context of the elements
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        /// <summary>
        /// Create the database on the server if not exits
        /// </summary>
        /// <returns>A task to monitor the progress</returns>
        protected async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Create the collection if not exits
        /// </summary>
        /// <returns>A task to monitor the progress</returns>
        protected async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
