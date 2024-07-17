using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Data;
using api.Exceptions;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDBContext _context;
        public CourseRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Course?> CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetByName(string name)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(course => course.Name.Equals(name));
            if (course == null) {
                throw new AppException($"Course not found with {name}", (int)HttpStatusCode.NotFound);
            }
            return course;
        }
    }
}