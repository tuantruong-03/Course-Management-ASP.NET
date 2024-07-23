using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Requests
{
    public class FacebookLoginRequest
    {
        [Required(ErrorMessage ="Token must be not null")]
        public string Token { get; set; } = string.Empty;
    }
}