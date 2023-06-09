﻿using Microsoft.AspNetCore.Mvc;

namespace USca_Server.Users
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut]
        public ActionResult<User> Login(LoginDTO loginCredentials)
        {
            var user = _userService.Login(loginCredentials);
            if (user == null)
            {
                return NotFound();
            }
            return StatusCode(200, new { username = user.Username });
        }
    }
}
