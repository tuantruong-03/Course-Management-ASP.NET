using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository) 
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers() {
            List<User> users = await userRepository.GetAllAsync();
            return Ok(users);
        }
        [HttpGet]
        [Route("{userName}")]
        public async Task<IActionResult> GetAllUsers([FromRoute] string userName) {
            User user = await userRepository.GetByUserName(userName);
            return Ok(user);
        }
        
    }
}