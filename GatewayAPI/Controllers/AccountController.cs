using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using GatewayAPI.Models.Account;
using GatewayAPI.Services;
using Listable.UserMicroservice.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GatewayAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult AccountLoginCheck()
        {
            var response = _userService.CheckForUserEntry(GetUserSub()).Result;

            if (response.IsSuccessStatusCode)
                return Ok();

            response = _userService.CreateUser(new UserDetails()
            {
                SubjectId = GetUserSub(),
                DisplayName = GenerateDisplayName()
            }).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetProfile(int userId)
        {
            var response = _userService.GetUser(userId).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var userDetails = JsonConvert.DeserializeObject<UserDetails>(response.Content.ReadAsStringAsync().Result);

            return Ok(new ProfileView()
            {
                Id = userDetails.Id,
                DisplayName = userDetails.DisplayName,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                DisplayPrivateProfile = false
            });
        }

        [HttpGet]
        public IActionResult GetProfileForAuthenticatedUser()
        {
            var response = _userService.GetUserBySub(GetUserSub()).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var userDetails = JsonConvert.DeserializeObject<UserDetails>(response.Content.ReadAsStringAsync().Result);

            return Ok(new ProfileView()
            {
                Id = userDetails.Id,
                DisplayName = userDetails.DisplayName,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                DisplayPrivateProfile = true
            });
        }

        [HttpPost]
        public IActionResult EditProfile([FromBody] EditProfileFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _userService.GetUserBySub(GetUserSub()).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var authenticatedUser = JsonConvert.DeserializeObject<UserDetails>(response.Content.ReadAsStringAsync().Result);

            if (model.Id != authenticatedUser.Id)
                return Forbid();

            response = _userService.UpdateUser(new UserDetails()
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                FirstName = model.FirstName,
                LastName = model.LastName
            }).Result;

            if(!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpGet]
        public IActionResult CheckDisplayName(string displayName)
        {
            if (displayName == null)
                return BadRequest();

            var response = _userService.CheckDisplayName(displayName).Result;

            if (!response.IsSuccessStatusCode)
                return BadRequest();

            return Ok();
        }

        private string GetUserSub()
        {
            string sub = "";

            foreach (var identity in User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            return sub;
        }

        private string GenerateDisplayName()
        {
            string name = "";
            int size = 0;

            StringLengthAttribute strLenAttr = typeof(UserDetails).GetProperty("DisplayName").GetCustomAttributes(typeof(StringLengthAttribute), false).Cast<StringLengthAttribute>().SingleOrDefault();
            if (strLenAttr != null)
                size = strLenAttr.MaximumLength;

            foreach (var identity in User.Identities)
            {
                name = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault().Value;
            }

            name = Regex.Replace(name, @"[^0-9a-zA-Z]+", "");

            name = name.Length < size-10 ? name : name.Substring(0, size-10);

            name = name + Guid.NewGuid().ToString("n").Substring(0, 10);

            var response = _userService.CheckDisplayName(name).Result;

            if (response.IsSuccessStatusCode)
                return name;
            else
                return GenerateDisplayName();
        }
    }
}
