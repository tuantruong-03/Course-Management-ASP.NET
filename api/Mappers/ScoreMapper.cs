using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.DTOs.Responses;
using api.Models;

namespace api.Mappers
{
    public static class ScoreMapper
    {   
        public static ScoreResponse ToResponseFromModel (this Score score) {
            return new ScoreResponse {
                CourseId = score.Course.Id,
                UserId = score.User.Id,
                UserName = score.User.UserName,
                CourseName = score.Course.Name,
                Value = score.Value,
            };
        }
        public static Score ToModelFromRequest(this ScoreRequest scoreRequest, User user, Course course) {
            return new Score {
                UserId = scoreRequest.UserId,
                CourseId = scoreRequest.CourseId,
                User = user,
                Course = course,
                Value = scoreRequest.Value,
            };
        }
    }
}