using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Requests;
using api.DTOs.Responses;
using api.Exceptions;
using api.Helpers;
using api.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JwtUtil jwtUtill;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly GoogleTokenValidator googleTokenValidator;
        private readonly FacebookTokenValidator facebookTokenValidator;
        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager,
RoleManager<IdentityRole> roleManager, JwtUtil jwtUtill, GoogleTokenValidator googleTokenValidator, FacebookTokenValidator facebookTokenValidator)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.jwtUtill = jwtUtill;
            this.googleTokenValidator = googleTokenValidator;
            this.facebookTokenValidator = facebookTokenValidator;
        }
        public async Task<User?> CreateAsync(User user)
        {
            User? existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new AppException($"User not found with ID {user.Id}", (int)HttpStatusCode.NotFound);
            }
            // Not yet implemented
            return null;
        }



        public async Task<List<User>> GetAllAsync()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<User> GetByUserName(string userName)
        {
            User? existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(userName));
            if (existingUser == null)
            {
                throw new AppException($"User not found with {userName}", (int)HttpStatusCode.NotFound);
            }
            return existingUser;
        }

        public async Task<List<Course>> GetCoursesOfUser(string userName)
        {
            User? existingUser = await userManager.Users.Include(user => user.CourseUser)
            .ThenInclude(courseUser => courseUser.Course).FirstOrDefaultAsync(u => u.UserName.Equals(userName));
            if (existingUser == null)
            {
                throw new AppException($"User not found with {userName}", (int)HttpStatusCode.NotFound);
            }
            List<Course> courses = existingUser.CourseUser.Select(courseUser => courseUser.Course).ToList();
            return courses;
        }

        public async Task<List<string>> GetRoles(User user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<List<Score>> GetScoresOfUserAsync(string userName)
        {
            var user = await userManager.Users
                .Include(user => user.Scores)
                    .ThenInclude(score => score.Course)
            .FirstOrDefaultAsync(user => user.UserName.ToLower().Equals(userName.ToLower()));
            if (user == null)
            {
                throw new AppException($"User not found with {userName}", (int)HttpStatusCode.NotFound);
            }
            return user.Scores;
        }

        public async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            GoogleJsonWebSignature.Payload payload = await googleTokenValidator.ValidateAsync(request.Token);


            User? existedUser = await userManager.FindByEmailAsync(payload.Email);
            if (existedUser == null)
            {
                User user = new User
                {
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                    Provider = Provider.Google,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)), (int)HttpStatusCode.BadRequest);
                }

                await userManager.AddToRoleAsync(user, "Student");
                string token = jwtUtill.GenerateJwtToken(user);
                return new LoginResponse
                {
                    AccessToken = token
                };
            }
            string jwtToken = jwtUtill.GenerateJwtToken(existedUser);
            return new LoginResponse
            {
                AccessToken = jwtToken
            };
        }

        public async Task<LoginResponse> FacebookLoginAsync(FacebookLoginRequest request)
        {
            var payload = await facebookTokenValidator.ValidateAndGetPayloadAsync(request.Token);
            System.Console.WriteLine(payload);
            User? existedUser = await userManager.FindByEmailAsync(payload["email"]?.ToString());
            if (existedUser == null)
            {
                User user = new User
                {
                    UserName = payload["email"]?.ToString(),
                    FirstName = payload["first_name"]?.ToString(),
                    LastName = payload["last_name"]?.ToString(),
                    Email = payload["email"]?.ToString(),
                    Provider = Provider.Facebook,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)), (int)HttpStatusCode.BadRequest);
                }

                await userManager.AddToRoleAsync(user, "Student");
                string token = jwtUtill.GenerateJwtToken(user);
                return new LoginResponse
                {
                    AccessToken = token
                };
            }
            string jwtToken = jwtUtill.GenerateJwtToken(existedUser);
            return new LoginResponse
            {
                AccessToken = jwtToken
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null)
            {
                throw new AppException($"Invalid username or password", (int)HttpStatusCode.BadRequest);
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: false);
            // lockoutOnFailure" parameter in the CheckPasswordSignInAsync method specifies whether a user should be locked out after a certain number of failed login attempts.
            if (!result.Succeeded)
            {

                throw new AppException($"Invalid username or password", (int)HttpStatusCode.BadRequest);
            }
            string token = jwtUtill.GenerateJwtToken(user);
            return new LoginResponse
            {
                AccessToken = token,
            };
        }

        public async Task<User?> RegisterAsync(RegisterRequest registerRequest)
        {
            bool isExistedUser = await userManager.Users.AnyAsync(user => user.UserName.ToLower().Equals(registerRequest.UserName.ToLower()));
            if (isExistedUser)
            {
                throw new AppException($"User existed with '{registerRequest.UserName}'", (int)HttpStatusCode.Conflict);
            }
            isExistedUser = await userManager.Users.AnyAsync(user => user.Email.Equals(registerRequest.Email));
            if (isExistedUser)
            {
                throw new AppException($"User existed with '{registerRequest.UserName}'", (int)HttpStatusCode.Conflict);
            }
            User user = new User
            {
                UserName = registerRequest.UserName,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                Provider = Provider.Local,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                // Field passwordHashed is generated by userManager
            };
            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)), (int)HttpStatusCode.BadRequest);
            }
            await userManager.AddToRoleAsync(user, "Student");
            return user;
        }
    }
}