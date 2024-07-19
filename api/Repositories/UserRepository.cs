using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Data;
using api.Exceptions;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;
        public UserRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
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
            if (user == null) {
                throw new AppException($"User not found with {userName}", (int)HttpStatusCode.NotFound);
            }
            return user.Scores;
        }
    }
}