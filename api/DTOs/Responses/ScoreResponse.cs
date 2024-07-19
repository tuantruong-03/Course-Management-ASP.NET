using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Responses
{
    public class ScoreResponse
    {
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string CourseName {get; set;} = string.Empty;
        public double Value {get; set;}

    }
}