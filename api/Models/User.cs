using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace api.Models
{
    public enum Provider {
        Local // 0
        ,Google // 1
        ,Facebook // 2
    }
    [Table("Users")]

    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Provider Provider {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        // Many to many
        public List<CourseUser> CourseUser { get; set; } = [];
        public List<Score> Scores { get; set; } = [];

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}