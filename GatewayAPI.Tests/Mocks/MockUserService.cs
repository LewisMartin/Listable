using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GatewayAPI.Services;
using Listable.UserMicroservice.DTO;
using Listable.UserMicroservice.Entities;
using Newtonsoft.Json;

namespace GatewayAPI.Tests.Mocks
{
    class MockUserService : IUserService
    {
        public List<User> DummyUsers { get; set; }

        public MockUserService(List<User> dummyUsers)
        {
            DummyUsers = dummyUsers;
        }

        public Task<HttpResponseMessage> CheckDisplayName(string displayName)
        {
            if(DummyUsers.Any(u => u.DisplayName == displayName))
                return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest });

            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> CheckForUserEntry(string subjectId)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> CreateUser(UserDetails userDetails)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> DeleteUser(int userId)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> GetUser(int userId)
        {
            var user = DummyUsers.Where(u => u.Id == userId).FirstOrDefault();

            if (user == null)
                return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError });

            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(user))
            });
        }

        public Task<HttpResponseMessage> GetUserBySub(string subjectId)
        {
            var user = DummyUsers.Where(u => u.SubjectId == subjectId).FirstOrDefault();

            if (user == null)
                return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.InternalServerError });

            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(user))
            });
        }

        public Task<HttpResponseMessage> UpdateUser(UserDetails userDetails)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }
    }
}
