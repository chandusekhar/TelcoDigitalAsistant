namespace Microsoft.Telco.DigitalAssistant.Data.Model
{
    using System;

    /// <summary>
    /// Represent the LostModel entity
    /// </summary>
    public class LostModelItem : Entity
    {
        /// <summary>
        /// Gets or sets when the device was lost
        /// </summary>
        public DateTime When { get; set; }
    }
}
