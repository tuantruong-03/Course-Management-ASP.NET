using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage ="Username must not be null")]
        [MinLength(1, ErrorMessage = "Username must not be blank")]
        public string UserName { get; set; } = string.Empty;
        // Let signInManager handle password validation
        public string Password { get; set; } = string.Empty;
    }
}