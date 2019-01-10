using System.Net.Http;
using System.Threading.Tasks;
using Listable.UserMicroservice.DTO;

namespace GatewayAPI.Services
{
    public interface IUserService
    {
        Task<HttpResponseMessage> GetUser(int userId);

        Task<HttpResponseMessage> CreateUser(UserDetails userDetails);

        Task<HttpResponseMessage> UpdateUser(UserDetails userDetails);

        Task<HttpResponseMessage> DeleteUser(int userId);

        Task<HttpResponseMessage> CheckDisplayName(string displayName);

        Task<HttpResponseMessage> CheckForUserEntry(string subjectId);
    }
}
