using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Mappers;
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
            
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }
        [HttpGet]
        [Route("{userName}")]
        public async Task<IActionResult> GetAllUsers([FromRoute] string userName) {
            User user = await userRepository.GetByUserName(userName);
            return Ok(user.ToResponseFromModel());
        }
        [HttpGet]
        [Route("{userName}/courses")]
        public async Task<IActionResult> GetCoursesOfUser([FromRoute] string userName) {
            List<Course> courses = await userRepository.GetCoursesOfUser(userName);
            return Ok(courses.Select(course => course.ToResponseFromModel()));
        }

        [HttpGet]
        [Route("{userName}/scores")]
        public async Task<IActionResult> GetScoresOfUser([FromRoute] string userName) {
            List<Score> scores = await userRepository.GetScoresOfUserAsync(userName);
            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }
        
    }
}