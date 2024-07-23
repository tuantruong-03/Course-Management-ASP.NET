using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.DTOs.Responses;
using api.Filters;
using api.Mappers;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public AuthController(IUserRepository userRepository) 
        {
            this.userRepository = userRepository;
        }
        [HttpPost]
        [Route("/login")]
        [ValidateModelState]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest) {
            LoginResponse loginResponse = await userRepository.LoginAsync(loginRequest);
            return Ok(loginResponse);
        }
        [HttpPost]
        [Route("/google-login")]
        [ValidateModelState]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest  request) {
            LoginResponse loginResponse = await userRepository.GoogleLoginAsync(request);
            return Ok(loginResponse);
        }
        [HttpPost]
        [Route("/facebook-login")]
        [ValidateModelState]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginRequest  request) {
            LoginResponse loginResponse = await userRepository.FacebookLoginAsync(request);
            return Ok(loginResponse);
        }
        [HttpPost]
        [Route("/register")]
        [ValidateModelState]
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest) {
            User? user = await userRepository.RegisterAsync(registerRequest);
            return Ok(user.ToResponseFromModel());
        }
    }
}