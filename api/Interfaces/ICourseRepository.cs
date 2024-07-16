using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface ICourseRepository
    {
        public Task<List<Course>> GetAllAsync();
        public Task<Course?> CreateAsync(Course course);
    }
}