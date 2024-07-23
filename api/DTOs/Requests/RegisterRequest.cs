using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Validations;

namespace api.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage ="Username must not be null")]
        [MinLength(1, ErrorMessage = "Username must not be blank")]
        public string UserName { get; set; } = string.Empty;
        // Let userManager handle password validation

        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [RegexValidation(@"([A-Z][a-z]*)", ErrorMessage = "First letter  of first name must be capitalized!")]
        public string FirstName { get; set; } = string.Empty;
        [RegexValidation(@"([A-Z][a-z]*)", ErrorMessage = "First letter  of first name must be capitalized!")]

        public string LastName { get; set; } = string.Empty;
    }
}