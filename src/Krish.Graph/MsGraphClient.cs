using Microsoft.Graph;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Krish.Graph
{
    public class MsGraphClient : IMsGraphClient
    {
        public ContactStatistics CreationStatistics => new ContactStatistics();

        public ContactStatistics DeletionStatistics => new ContactStatistics();

        public FilterOptions FilterOptions => new FilterOptions();

        private IGraphServiceClient GraphClient { get; set; }

        private readonly int maxRetries = 10;

        public MsGraphClient(IGraphServiceClient graphServiceClient)
        {
            this.GraphClient = graphServiceClient;
        }

        public async Task<User> GetMeAsync()
        {
            return await GraphClient
                .Me
                .Request()
                .Select(Constants.UserPropertiesToLoad)
                .GetAsync();
        }

        public async Task<byte[]> GetMyPhotoAsync()
        {
            // Outlook and mobile clients don't display the photo correctly if the size is above 200 pixels.
            // So get the maximum size below 200 pixels.
            var photoSizes = await GraphClient.Me.Photos.Request().GetAsync();
            var size = photoSizes.Where(ps => ps.Height < 200 && ps.Width < 200).OrderByDescending(ps => ps.Height).FirstOrDefault();
            if (size != default(ProfilePhoto))
            {
                var photo = await GraphClient.Me.Photos[size.Id].Content.Request().GetAsync();
                using (photo)
                {
                    return ((MemoryStream)photo).ToArray();
                }
            }
            return default(byte[]);
        }


        public async Task<long> GetColleaguesNearMeCountAsync()
        {
            var usersToBeAdded = await GraphClient.Users
                .Request(new[] { new QueryOption(Constants.Count_Name, Constants.True_Lowercase) })
                .Header(Constants.ConsistencyLevel_Name, Constants.ConsistencyLevel_Value)
                .Filter(await CreateUserFilter(FilterOptions))
                .Select(Constants.Id_Name)
                .Top(1)
                .GetAsync();
            CreationStatistics.Total = (long)usersToBeAdded.AdditionalData[Constants.OdataCount_Name];
            return CreationStatistics.Total;
        }


        public async Task<long> GetContactsInO365CountAsync()
        {
            var contacts = await GraphClient.Me.Contacts
                .Request(new[] { new QueryOption(Constants.Count_Name, Constants.True_Lowercase) })
                .Header(Constants.ConsistencyLevel_Name, Constants.ConsistencyLevel_Value)
                .Select(Constants.Id_Name)
                .Top(1)
                .GetAsync();
            return (long)contacts.AdditionalData[Constants.OdataCount_Name];
        }

        public async Task<long> GetExistingContactsNearMeCountAsync()
        {
            var contacts = await GraphClient.Me.Contacts
                .Request(new[] { new QueryOption(Constants.Count_Name, Constants.True_Lowercase) })
                .Header(Constants.ConsistencyLevel_Name, Constants.ConsistencyLevel_Value)
                .Filter(await CreateContactFilter())
                .Select(Constants.Id_Name)
                .Top(1)
                .GetAsync();
            DeletionStatistics.Total = (long)contacts.AdditionalData[Constants.OdataCount_Name];
            return DeletionStatistics.Total;
        }

        public async Task AddContactsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteExistingContactsNearMeAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<string> CreateUserFilter(FilterOptions filterOptions)
        {
            var user = await GetMeAsync();
            string filterString = $"{nameof(user.City)} eq '{user.City}'";
            if (filterOptions.UseCompanyName)
            {
                filterString += $" and {nameof(user.CompanyName)} eq '{user.CompanyName}'";
            }
            if (filterOptions.UseOfficeLocation)
            {
                filterString += $" and {nameof(user.OfficeLocation)} eq '{user.OfficeLocation}'";
            }
            return filterString;
        }

        private async Task<string> CreateContactFilter()
        {
            var user = await GetMeAsync();
            return $"{nameof(user.OfficeLocation)} eq '{user.OfficeLocation}'";
        }
    }
}
