namespace Microsoft.Telco.DigitalAssistant.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Telco.DigitalAssistant.Data.Model;

    /// <summary>
    /// This class will handle how to persist Dialog Metrics
    /// </summary>
    public class DialogMetricManager : IDisposable
    {
        /// <summary>
        /// Private reference to the Document DB Context for the DialogMetric Entity
        /// </summary>
        private DocumentDBRepository<DialogMetric> context = new DocumentDBRepository<DialogMetric>();

        /// <summary>
        /// Gets the DialogMetric for a given dialog id
        /// </summary>
        /// <param name="value">Dialog Id</param>
        /// <returns>The <see cref="DialogMetric"/> reference</returns>
        public async Task<DialogMetric> GetDialogMetricFromId(string value)
        {
            DialogMetric result = null;

            if (!string.IsNullOrEmpty(value))
            {
                result = (await context.GetItemsAsync(p => p.Id == value)).FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// Create a new <see cref="DialogMetric"/>
        /// </summary>
        /// <param name="value">A reference to the item</param>
        /// <returns>The newly inserted element</returns>
        public async Task<DialogMetric> CreateDialogMetric(DialogMetric value)
        {
            return await context.CreateItemAsync(value);
        }

        /// <summary>
        /// Update the <see cref="DialogMetric"/>
        /// </summary>
        /// <param name="value">A reference to the item</param>
        /// <returns>The updated element</returns>
        public async Task<DialogMetric> UpdateDialogMetric(DialogMetric value)
        {
            DialogMetric result = null;

            result = await context.UpdateItemAsync(value.Id, value);

            return result;
        }

        /// <summary>
        /// Dispose the context
        /// </summary>
        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }
    }
}
