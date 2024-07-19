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

namespace api.Interfaces
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> userManager;
        public ScoreRepository(ApplicationDBContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            _context = context;
        }

        public async Task<Score> CreateOrUpdateAsync(ScoreRequest scoreRequest)
        {
            int courseId = scoreRequest.CourseId;
            string userId = scoreRequest.UserId;
            double value = scoreRequest.Value;
            var course = await _context.Courses.Include(course => course.Scores).FirstOrDefaultAsync(course => course.Id == courseId);
            if (course == null)
            {
                throw new AppException($"Course not found with ID '{courseId}'", (int)HttpStatusCode.NotFound);
            }
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new AppException($"User not found with ID '{userId}'", (int)HttpStatusCode.NotFound);
            }
            Score? existingScore = await _context.Scores.FirstOrDefaultAsync(score => score.CourseId == courseId && score.UserId.Equals(userId));
            if (existingScore != null)
            {
                existingScore.Value = value;
                await _context.SaveChangesAsync();
                return existingScore;
            }
            Score newScore = scoreRequest.ToModelFromRequest(user, course);
            course.Scores.Add(newScore);
            await _context.SaveChangesAsync();
            return newScore;
        }

        public async Task<List<Score>> GetAllAsync()
        {
            return await _context.Scores.Include(score => score.User).Include(score => score.Course).ToListAsync();
        }

        public async Task RemoveAsync(ScoreDeleteRequest scoreDeleteRequest)
        {
            int courseId = scoreDeleteRequest.CourseId;
            string userId = scoreDeleteRequest.UserId;
            var course = await _context.Courses.Include(course => course.Scores).FirstOrDefaultAsync(course => course.Id == courseId);
            if (course == null)
            {
                throw new AppException($"Course not found with ID '{courseId}'", (int)HttpStatusCode.NotFound);
            }
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new AppException($"User not found with ID '{userId}'", (int)HttpStatusCode.NotFound);
            }
            Score? score = await _context.Scores.FirstOrDefaultAsync(score => score.CourseId == courseId && score.UserId.Equals(userId));
            if (score == null)
            {
                throw new AppException($"Student is not graded", (int)HttpStatusCode.BadRequest);
            }
            _context.Scores.Remove(score);
            await _context.SaveChangesAsync();
        }
    }
}