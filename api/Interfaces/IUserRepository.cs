using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllAsync();
        public Task<User?> CreateAsync(User user);
        public Task<List<Score>> GetScoresOfUserAsync(string userName);

        public Task<User> GetByUserName(string userName);
        public Task<List<Course>> GetCoursesOfUser(string userName); 
        public Task<List<string>> GetRoles(User user);
    }
}