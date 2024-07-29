using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Requests;
using api.Exceptions;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> userManager;
        public CourseRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }
        public async Task<Course?> CreateAsync(CourseCreateRequest courseCreateRequest)
        {
            string courseName = courseCreateRequest.Name;
            if (_context.Courses.Any(course => course.Name.ToLower().Equals(courseName.ToLower())))
            {
                throw new AppException($"Course existed with '{courseName}'", (int)HttpStatusCode.Conflict);
            }
            Course course = courseCreateRequest.ToModelFromCreateRequest();
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task RemoveAsync(int id)
        {
            Course? existingCourse = await _context.Courses.FirstOrDefaultAsync(course => course.Id == id);
            if (existingCourse == null)
            {
                throw new AppException($"Course not found with Id {id}", (int)HttpStatusCode.NotFound);
            }
            _context.Courses.Remove(existingCourse);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetByNameAsync(string name)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(course => course.Name.ToLower().Equals(name.ToLower()));
            if (course == null)
            {
                throw new AppException($"Course not found with '{name}'", (int)HttpStatusCode.NotFound);
            }
            return course;
        }


        public async Task<List<User>?> GetUsersOfCourseAsync(string name, string? role)
        {
            var course = await _context.Courses.Include(course => course.CourseUser)
            .ThenInclude(courseUser => courseUser.User)
            .FirstOrDefaultAsync(course => course.Name.Equals(name));
            if (course == null)
            {
                throw new AppException($"Course not found with '{name}'", (int)HttpStatusCode.NotFound);
            }
            var users = course.CourseUser.Select(cu => cu.User).ToList();
            if (role != null)
            {
                List<User> usersWithRole = new List<User>();
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Contains(role))
                    {
                        usersWithRole.Add(user);
                    }
                }
                users = usersWithRole;
            }
            return users;
        }

        public async Task<Course?> UpdateAsync(int courseId, CourseCreateRequest courseCreateRequest)
        {
            Course? existingCourse = await _context.Courses.FirstOrDefaultAsync(course => course.Id == courseId);
            if (existingCourse == null)
            {
                throw new AppException($"Course not found with Id {courseId}", (int)HttpStatusCode.NotFound);
            }
            bool isNotUniqueName = await _context.Courses.AnyAsync(course => course.Name.ToLower().Equals(courseCreateRequest.Name.ToLower()));
            if (isNotUniqueName)
            {
                throw new AppException($"Course with name '{courseCreateRequest.Name}' existed", (int)HttpStatusCode.Conflict);
            }
            existingCourse.Name = courseCreateRequest.Name;
            existingCourse.MaxNumberOfStudents = courseCreateRequest.MaxNumberOfStudents;
            existingCourse.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingCourse;
        }

        public async Task<CourseUser> AddUserToCourseAsync(CourseUserCreateRequest courseUserCreateRequest)
        {
            int courseId = courseUserCreateRequest.CourseId;
            string userId = courseUserCreateRequest.UserId;
            var course = await _context.Courses.Include(course => course.CourseUser).FirstOrDefaultAsync(course => course.Id == courseId);
            if (course == null)
            {
                throw new AppException($"Course not found with ID '{courseId}'", (int)HttpStatusCode.NotFound);
            }
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new AppException($"User not found with ID '{userId}'", (int)HttpStatusCode.NotFound);
            }
            if (course.CourseUser.Any(courseUser => courseUser.UserId.Equals(userId)))
            {
                throw new AppException($"Student is already enrolled in this course.", (int)HttpStatusCode.Conflict);
            }
            // Add student to course
            CourseUser courseUser = new CourseUser
            {
                UserId = userId,
                CourseId = courseId,
                Course = course,
                User = user,
            };
            course.CourseUser.Add(courseUser);
            await _context.SaveChangesAsync();
            return courseUser;

        }

        public async Task RemoveUserFromCourseAsync(CourseUserDeleteRequest courseUserDeleteRequest)
        {
            int courseId = courseUserDeleteRequest.CourseId;
            string userId = courseUserDeleteRequest.UserId;
            var course = await _context.Courses.Include(course => course.CourseUser).FirstOrDefaultAsync(course => course.Id == courseId);
            if (course == null)
            {
                throw new AppException($"Course not found with ID '{courseId}'", (int)HttpStatusCode.NotFound);
            }
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new AppException($"User not found with ID '{userId}'", (int)HttpStatusCode.NotFound);
            }
            CourseUser? courseUser = course.CourseUser.FirstOrDefault(courseUser => courseUser.UserId.Equals(userId));
            if (courseUser == null)
            {
                throw new AppException($"Student is not enrolled in this course", (int)HttpStatusCode.BadRequest);
            }
            // Remove the student from course
            course.CourseUser.Remove(courseUser);
            await _context.SaveChangesAsync();

        }

        public async Task<List<Score>?> GetScoresOfCourseAsync(string name)
        {
            var course = await _context.Courses
            .Include(course => course.Scores)
                .ThenInclude(score => score.User)
            .FirstOrDefaultAsync(course => course.Name.Equals(name));
            if (course == null)
            {
                throw new AppException($"Course not found with '{name}'", (int)HttpStatusCode.NotFound);
            }
            var scores = course.Scores;
            return scores;
        }

        public async Task<List<Score>> ImportScoresFromExcelAsync(string courseName, IFormFile file)
        {
            Course? existingCourse = await _context.Courses.FirstOrDefaultAsync(course => course.Name.Equals(courseName));
            if (existingCourse == null)
            {
                throw new AppException($"Course not found with '{courseName}'", (int)HttpStatusCode.NotFound);
            }
            try
            {
                var transaction = await _context.Database.BeginTransactionAsync();
                MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                ExcelPackage excelPackage = new ExcelPackage(memoryStream);
                ExcelWorksheet? worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new AppException("No worksheet found in the uploaded Excel file.", (int)HttpStatusCode.BadRequest);
                }
                int row = worksheet.Dimension.Rows;
                List<Score> scores = new List<Score>();
                for (int i = 2; i <= row; ++i)
                {
                    string? username = worksheet.Cells[i, 1].Value?.ToString();
                    string? valueString = worksheet.Cells[i, 2].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(valueString))
                    {
                        continue; // Skip rows with empty username or email
                    }
                    bool isValidValue = int.TryParse(valueString, out int value);
                    if (!isValidValue || value < 0 || value > 10)
                    {
                        System.Console.WriteLine("Invalid score");
                        continue;
                    }

                    User? existingUser = await userManager.Users.FirstOrDefaultAsync(user => user.UserName.Equals(username));
                    if (existingUser == null)
                    {
                        await transaction.RollbackAsync();
                        throw new AppException($"User not found with '{username}'", (int)HttpStatusCode.NotFound);
                    }
                    Score? existingScore = await _context.Scores.FirstOrDefaultAsync(score => score.UserId.Equals(existingUser.Id) && score.CourseId.Equals(existingCourse.Id));
                    if (existingScore != null)
                    {
                        // Score is existed, updaate value of score
                        existingScore.Value = value;
                        await _context.SaveChangesAsync();
                        scores.Add(existingScore);
                    }
                    else
                    {
                        Score score = new Score
                        {
                            Course = existingCourse,
                            User = existingUser,
                            UserId = existingUser.Id,
                            CourseId = existingCourse.Id,
                            Value = value
                        };
                        await _context.Scores.AddAsync(score);
                        await _context.SaveChangesAsync();
                        scores.Add(score);
                    }

                }
                await transaction.CommitAsync();
                return scores;

            }
            catch (Exception e)
            {
                throw new AppException(e.InnerException?.Message ?? e.Message, (int)HttpStatusCode.InternalServerError);
            }

        }

        public async Task<byte[]> ExportScoresToExcelAsync(string courseName)
        {
            List<Score> scores = await _context.Scores
                .Include(score => score.Course)
                .Include(score => score.User)
                .Where(score => score.Course.Name.Equals(courseName)).ToListAsync();
            ExcelPackage excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Scores");
            worksheet.Cells[1, 1].Value = "UserName";
            worksheet.Cells[1, 2].Value = "Value";
            for (int i = 0; i < scores.Count; ++i) {
                worksheet.Cells[i+2,1].Value = scores[i].User.UserName;
                worksheet.Cells[i+2,2].Value = scores[i].Value;
            }
            return await excelPackage.GetAsByteArrayAsync();
        }
    }
}