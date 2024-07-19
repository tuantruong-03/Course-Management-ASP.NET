using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.Models;

namespace api.Repositories
{
    public interface ICourseRepository
    {
        public Task<List<Course>> GetAllAsync();
        public Task<Course?> GetByNameAsync(string name);
        public Task<Course?> CreateAsync(CourseCreateRequest courseCreateRequest);
        public Task<Course?> UpdateAsync(int id, CourseCreateRequest courseCreateRequest);
        public Task RemoveAsync(int id);

        public Task<List<User>?> GetUsersOfCourseAsync(string name, string? role);
        public Task<List<Score>?> GetScoresOfCourseAsync(string name);
        public Task<CourseUser> AddUserToCourseAsync(CourseUserCreateRequest courseUserCreateRequest);
        public Task RemoveUserFromCourseAsync(CourseUserDeleteRequest courseUserDeleteRequest);
    }
}