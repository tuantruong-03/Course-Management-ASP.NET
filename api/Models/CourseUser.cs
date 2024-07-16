using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    [PrimaryKey(nameof(UserId), nameof(CourseId))]
    [Table("CourseUser")]
    public class CourseUser
    {
        public int CourseId { get; set; }
        public string? UserId { get; set; }
        public Course Course { get; set; } 
        public User User { get; set; }
    }
}