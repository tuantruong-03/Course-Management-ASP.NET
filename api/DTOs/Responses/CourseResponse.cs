using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.response
{
    public class CourseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxNumberOfStudents { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}