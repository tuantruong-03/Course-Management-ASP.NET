using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.DTOs.response;
using api.Models;

namespace api.Mappers
{
    public static class CourseMapper
    {
        public static CourseResponse ToResponseFromModel(this Course course) {
            return new CourseResponse {
                Id = course.Id,
                Name = course.Name,
                MaxNumberOfStudents = course.MaxNumberOfStudents,
                CreatedAt = course.CreatedAt,
                ModifiedAt = course.ModifiedAt,
            };
        }
        public static Course ToModelFromCreateRequest(this CourseCreateRequest courseCreateRequest) {
            return new Course {
                Name = courseCreateRequest.Name,
                MaxNumberOfStudents = courseCreateRequest.MaxNumberOfStudents,
            };
        }
    }
}