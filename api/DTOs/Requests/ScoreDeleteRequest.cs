using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
{
    public class ScoreDeleteRequest
    {
        [Required(ErrorMessage ="UserId is requried")]
        public string UserId {get; set;} = string.Empty;
        [Required(ErrorMessage ="CourseId is requried")]
        public int CourseId {get; set;}
    }
}