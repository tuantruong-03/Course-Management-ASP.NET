using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
{
    // For update and create
    public class ScoreRequest
    {

        [Required(ErrorMessage ="UserId is requried")]
        public string UserId {get; set;} = string.Empty;
        [Required(ErrorMessage ="CourseId is requried")]
        public int CourseId {get; set;}
        [Range(0d, 10d,ErrorMessage = "Score must be from 0 to 10")]
        public double Value {get; set;}
    }
}