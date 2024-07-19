using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.response;
using api.Models;

namespace api.DTOs.Responses
{
    public class CourseUserResponse
    {
         public int CourseId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public CourseResponse Course { get; set; }
        public UserResponse User { get; set; }
    }
}