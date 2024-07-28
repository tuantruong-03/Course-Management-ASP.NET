using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Filters;
using api.Mappers;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
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
            string extension = Path.GetExtension(file.FileName).ToLower();
            // Read the header and validate it
            var isHeaderValid = await ValidateHeaderAsync(file, extension);
            if (!isHeaderValid)
            {
                return BadRequest("Invalid file header. The file must contain 'Name', 'Email', 'First Name', 'Last Name', and 'Roles' columns.");
            }

            List<User> users =  await userRepository.ImportUsersFromExcelAsync(file);
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }

        private async Task<bool> ValidateHeaderAsync(IFormFile file, string extension)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                if (extension == ".csv")
                {
                    using (var reader = new StreamReader(stream))
                    {
                        stream.Position = 0;
                        var headerLine = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(headerLine))
                        {
                            return false;
                        }

                        var headers = headerLine.Split(',');
                        return ValidateHeaders(headers);
                    }
                }
                else if (extension == ".xlsx")
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        var headers = new List<string>();

                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            System.Console.WriteLine(worksheet.Cells[1,col].Value?.ToString());
                            headers.Add(worksheet.Cells[1, col].Value?.ToString().Trim());
                        }

                        return ValidateHeaders(headers.ToArray());
                    }
                }
            }

            return false;
        }

        private bool ValidateHeaders(string[] headers)
        {
            var requiredHeaders = new[] { "UserName", "Email", "First Name", "Last Name", "Roles" };
            return headers.All(header => requiredHeaders.Contains(header, StringComparer.OrdinalIgnoreCase));
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