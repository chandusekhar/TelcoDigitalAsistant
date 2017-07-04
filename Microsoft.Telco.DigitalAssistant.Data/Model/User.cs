namespace Microsoft.Telco.DigitalAssistant.Data.Model
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// The user entity
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// Gets or sets the name of the user
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the user
        /// </summary>
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets when the user is created
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
    }
}
