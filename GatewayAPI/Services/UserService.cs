using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Listable.UserMicroservice.DTO;

namespace GatewayAPI.Services
{
    public enum UserApiAction
    {
        GetUser,
        GetUserBySub,
        CreateUser,
        UpdateUser,
        DeleteUser,
        CheckDisplayName,
        CheckForUserEntry
    }

    public class UserService : BackendService<UserApiAction>, IUserService
    {
        public UserService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor) { }

        public async Task<HttpResponseMessage> GetUser(int userId)
        {
            return await APIRequest(UserApiAction.GetUser, "?userId=" + userId);
        }

        public async Task<HttpResponseMessage> GetUserBySub(string subjectId)
        {
            return await APIRequest(UserApiAction.GetUserBySub, "subjectId=" + subjectId);
        }

        public async Task<HttpResponseMessage> CheckForUserEntry(string subjectId)
        {
            return await APIRequest(UserApiAction.CheckForUserEntry, "?subjectId=" + subjectId);
        }

        public async Task<HttpResponseMessage> CreateUser(UserDetails userDetails)
        {
            return await APIRequest(UserApiAction.CreateUser, "", new StringContent(JsonConvert.SerializeObject(userDetails).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateUser(UserDetails userDetails)
        {
            return await APIRequest(UserApiAction.UpdateUser, "", new StringContent(JsonConvert.SerializeObject(userDetails).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteUser(int userId)
        {
            return await APIRequest(UserApiAction.DeleteUser, "?userId=" + userId);
        }

        public async Task<HttpResponseMessage> CheckDisplayName(string displayName)
        {
            return await APIRequest(UserApiAction.CheckDisplayName, "?displayName=" + displayName);
        }

        protected override async Task<HttpResponseMessage> APIRequest(UserApiAction action, string uriParams = "", HttpContent content = null)
        {
            var req = CreateAPIRequestMessage(action, uriParams);

            if (action == UserApiAction.CreateUser || action == UserApiAction.UpdateUser)
                req.Content = content;

            string accessToken = await GetAccessTokenAsync(BackendAPI.UserAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        protected override HttpRequestMessage CreateAPIRequestMessage(UserApiAction action, string uriParams)
        {
            switch (action)
            {
                case UserApiAction.GetUser:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["UserServiceAPI:APIEndpoint"] + "/getuser" + uriParams));
                case UserApiAction.GetUserBySub:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["UserServiceAPI:APIEndpoint"] + "/getuserbysub" + uriParams));
                case UserApiAction.CheckForUserEntry:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["UserServiceAPI:APIEndpoint"] + "/checkforuserentry" + uriParams));
                case UserApiAction.CreateUser:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["UserServiceAPI:APIEndpoint"] + "/createuser" + uriParams));
                case UserApiAction.UpdateUser:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["UserServiceAPI:APIEndpoint"] + "/updateuser" + uriParams));
                case UserApiAction.DeleteUser:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["UserServiceAPI:APIEndpoint"] + "/deleteuser" + uriParams));
                case UserApiAction.CheckDisplayName:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["UserServiceAPI:APIEndpoint"] + "/checkdisplayname" + uriParams));
                default:
                    return new HttpRequestMessage();
            }
        }
    }
}
