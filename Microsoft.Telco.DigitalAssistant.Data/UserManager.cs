namespace Microsoft.Telco.DigitalAssistant.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Telco.DigitalAssistant.Data.Model;

    /// <summary>
    /// This class represent the User repository
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Gets the users from the database, if the user doesn´t exits it´s created
        /// </summary>
        /// <param name="id">Activity i</param>
        /// <param name="name">Name of the user</param>
        /// <param name="partitionId">Partition id, in this context is the channel id</param>
        /// <returns>A reference to the user</returns>
        public async Task<User> CheckUser(string id, string name, string partitionId)
        {
            User result = null;
            DocumentDBRepository<User> context = new DocumentDBRepository<User>();
            var found = await context.GetItemsAsync(p => p.Id == id);
            if (found != null && found.Count() == 0)
            {
                result = new User()
                {
                    Created = DateTime.UtcNow,
                    Id = id,
                    Name = name,
                    PartitionId = partitionId
                };
                await context.CreateItemAsync(result);
            }
            else
            {
                result = found.FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// Gets the user from id
        /// </summary>
        /// <param name="value">Id for the user</param>
        /// <returns>A reference to the user</returns>
        public async Task<User> GetUserById(string value)
        {
            DocumentDBRepository<User> context = new DocumentDBRepository<User>();
            var found = await context.GetItemsAsync(p => p.Id == value);
            return found.FirstOrDefault();
        }

        /// <summary>
        /// Update the user in the database
        /// </summary>
        /// <param name="value">A reference to the user</param>
        /// <returns>A task to monitor the progress</returns>
        public async Task SaveUser(User value)
        {
            DocumentDBRepository<User> context = new DocumentDBRepository<User>();
            await context.UpdateItemAsync(value.Id, value);
        }
    }
}
