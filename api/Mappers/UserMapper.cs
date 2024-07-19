using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.response;
using api.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponseFromModel(this User user) {
            return new UserResponse {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
            };
        }
    }
}