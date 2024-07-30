using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Filters;
using api.Helpers;
using api.Mappers;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    // [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<User> users = await userRepository.GetAllAsync();

            return Ok(users.Select(user => user.ToResponseFromModel()));
        }
        [HttpGet]
        [Route("export-to-excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            byte[] excelData = await userRepository.ExportUsersToExcelAsync();
            var fileName = "Users.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileDownloadName: fileName);
        }

        [HttpPost]
        [Route("import-from-excel")]
        [ValidateExcelFile("file")] // "file" is the parameter name
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            // Read the header and validate it
            var isHeaderValid = await ExcelValidator.ValidateUsersFile(file);
            if (!isHeaderValid)
            {
                return BadRequest("Invalid file header. The file must contain 'Name', 'Email', 'First Name', 'Last Name', and 'Roles' columns.");
            }

            List<User> users =  await userRepository.ImportUsersFromExcelAsync(file);
            users.ForEach(u => System.Console.WriteLine(u.ToString()));
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }


        [HttpGet]
        [Route("{userName}")]
        public async Task<IActionResult> GetAllUsers([FromRoute] string userName)
        {
            User user = await userRepository.GetByUserName(userName);
            return Ok(user.ToResponseFromModel());
        }
        [HttpGet]
        [Route("{userName}/courses")]
        public async Task<IActionResult> GetCoursesOfUser([FromRoute] string userName)
        {
            List<Course> courses = await userRepository.GetCoursesOfUser(userName);
            return Ok(courses.Select(course => course.ToResponseFromModel()));
        }

        [HttpGet]
        [Route("{userName}/scores")]
        public async Task<IActionResult> GetScoresOfUser([FromRoute] string userName)
        {
            List<Score> scores = await userRepository.GetScoresOfUserAsync(userName);
            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }

    }
}