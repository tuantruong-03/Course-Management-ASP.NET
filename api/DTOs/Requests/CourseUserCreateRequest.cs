using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
// This class is used for add, remove student from course
{
    public class CourseUserCreateRequest
    {
        [Required(ErrorMessage ="UserId is requried")]
        public string UserId {get;set;} = string.Empty;
        [Required(ErrorMessage ="CourseId is requried")]
        public int CourseId {get;set;}
    }
}