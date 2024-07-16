using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{   
    [PrimaryKey(nameof(UserId), nameof(CourseId))]
    [Table("Scores")]
    public class Score
    {
        public string UserId {get; set;}
        public User User {get; set;} 
        public int CourseId {get; set;}
        public Course Course {get; set;}
        public double Value {get; set;}
    }
}