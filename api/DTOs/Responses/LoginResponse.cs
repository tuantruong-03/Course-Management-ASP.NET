using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}