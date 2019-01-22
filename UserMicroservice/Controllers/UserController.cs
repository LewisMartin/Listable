using System;
using System.Linq;
using Listable.UserMicroservice.DTO;
using Listable.UserMicroservice.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listable.UserMicroservice.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private UserDbContext _DbContext { get; }

        public UserController(UserDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetUser(int userId)
        {
            var user = _DbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
                return Json(user);
            else
                return NotFound();
        }

        [HttpGet]
        public IActionResult GetUserBySub(string subjectId)
        {
            if (subjectId == null)
                return BadRequest();

            var user = _DbContext.Users.FirstOrDefault(u => u.SubjectId == subjectId);

            if (user != null)
                return Json(user);
            else
                return NotFound();
        }

        [HttpGet]
        public IActionResult CheckForUserEntry(string subjectId)
        {
            if (subjectId == null)
                return BadRequest();

            if (_DbContext.Users.Any(u => u.SubjectId == subjectId))
                return Ok();
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDetails userDetails)
        {
            if (userDetails == null)
                return BadRequest();

            try
            {
                _DbContext.Users.Add(new User()
                {
                    SubjectId = userDetails.SubjectId,
                    DisplayName = userDetails.DisplayName,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName
                });

                _DbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public IActionResult UpdateUser([FromBody] UserDetails userDetails)
        {
            if (userDetails == null)
                return BadRequest();

            try
            {
                var user = _DbContext.Users.FirstOrDefault(u => u.Id == userDetails.Id);

                user.DisplayName = userDetails.DisplayName;
                user.FirstName = userDetails.FirstName;
                user.LastName = userDetails.LastName;

                _DbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public IActionResult DeleteUser(int userId)
        {
            var user = _DbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            _DbContext.Users.Remove(user);
            _DbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult CheckDisplayName(string displayName)
        {
            if (_DbContext.Users.Any(u => u.DisplayName.ToLower() == displayName.ToLower()))
                return BadRequest();
            else
                return Ok();
        }
    }
}
