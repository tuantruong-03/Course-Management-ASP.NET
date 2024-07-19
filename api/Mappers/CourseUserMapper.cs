using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.response;
using api.DTOs.Responses;
using api.Models;

namespace api.Mappers
{
    public static class CourseUserMapper
    {
        public static CourseUserResponse ToResponseFromModel(this CourseUser courseUser) {
            return new CourseUserResponse {
                UserId = courseUser.UserId,
                CourseId = courseUser.CourseId,
                User = courseUser.User.ToResponseFromModel(),
                Course = courseUser.Course.ToResponseFromModel(),
            };
        }
    }
}