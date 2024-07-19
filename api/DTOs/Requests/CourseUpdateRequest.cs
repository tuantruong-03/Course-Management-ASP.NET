using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
{
    
    public class CourseUpdateRequest
    {
        [Required(ErrorMessage ="Name is requried")]
        [MinLength(1, ErrorMessage = "Name must not be empty")]
        public string Name { get; set;  } = string.Empty;
        [Range(20,40, ErrorMessage ="MaxNumberOfStudents must be between 20 and 40")]
        public int MaxNumberOfStudents { get; set;  }
    }
}