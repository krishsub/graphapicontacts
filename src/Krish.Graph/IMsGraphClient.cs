using Microsoft.Graph;
using System.Threading.Tasks;

namespace Krish.Graph
{
    public interface IMsGraphClient
    {
        /// <summary>
        /// Statistics that provides insights into creation of contacts.
        /// </summary>
        ContactStatistics CreationStatistics { get; }

        /// <summary>
        /// Statistics that provides insights into deletion of contacts.
        /// </summary>
        ContactStatistics DeletionStatistics { get; }

        /// <summary>
        /// The filter options to use when searching for org users (i.e. colleagues) to add to your contacts.
        /// </summary>
        FilterOptions FilterOptions { get; }

        /// <summary>
        /// Returns details about the currently logged in user.
        /// </summary>
        /// <returns>A user object representing the details of the logged in user.</returns>
        Task<User> GetMeAsync();

        /// <summary>
        /// Returns the photo of the currently logged in user.
        /// </summary>
        /// <returns>The photo as an array of bytes if present, null otherwise.</returns>
        Task<byte[]> GetMyPhotoAsync();

        /// <summary>
        /// Returns the estimated number of colleagues based on their location information.
        /// </summary>
        /// <returns>The estimated number of contacts to be created.</returns>
        Task<long> GetColleaguesNearMeCountAsync();

        /// <summary>
        /// Returns the estimated number of contacts in your Office 365 subscription.
        /// Note that the accuracy of the count can have a latency of seconds or minutes.
        /// </summary>
        /// <returns>The estimated number of contacts in Office 365 with seconds/minutes accuracy.</returns>
        Task<long> GetContactsInO365CountAsync();

        /// <summary>
        /// Returns the number of existing contacts who are located at the same office as you.
        /// These are candidates for a possible deletion when creating new contacts.
        /// Note that the accuracy of the count can have a latency of seconds or minutes.
        /// </summary>
        /// <returns>The estimated number of contacts in Office 365 with seconds/minutes accuracy.</returns>
        Task<long> GetExistingContactsNearMeCountAsync();

        /// <summary>
        /// Adds colleagues based on their location to your Office 365 contacts. Outlook and
        /// other client applications (including Outlook mobile) will eventually synchronize these 
        /// contacts locally.
        /// </summary>
        /// <returns>A task representing the long running operation.</returns>
        Task AddContactsAsync();

        /// <summary>
        /// Deletes your contacts who are located at the same office as you.
        /// </summary>
        /// <returns>A task representing the long running operation.</returns>
        Task DeleteExistingContactsNearMeAsync();
    }
}
