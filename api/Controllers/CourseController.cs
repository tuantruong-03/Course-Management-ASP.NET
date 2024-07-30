using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.Filters;
using api.Helpers;
using api.Mappers;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    // [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository courseRepository;
        public CourseController(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await courseRepository.GetAllAsync();
            return Ok(courses.Select(course => course.ToResponseFromModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseCreateRequest courseCreateRequest)
        {
            var course = await courseRepository.CreateAsync(courseCreateRequest);
            return Ok(course.ToResponseFromModel());

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseCreateRequest courseCreateRequest)
        {
            Course? course = await courseRepository.UpdateAsync(id, courseCreateRequest);
            return Ok(course.ToResponseFromModel());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await courseRepository.RemoveAsync(id);
            return NoContent();
        }

        [ValidateModelState]
        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> AddUserToCourse([FromBody] CourseUserCreateRequest courseUserCreateRequest)
        {
            CourseUser? courseUser = await courseRepository.AddUserToCourseAsync(courseUserCreateRequest);
            return Ok(courseUser.ToResponseFromModel());
        }

        [ValidateModelState]
        [HttpDelete]
        [Route("users")]
        public async Task<IActionResult> RemoveUserFromCourse([FromBody] CourseUserDeleteRequest courseUserDeleteRequest)
        {
            await courseRepository.RemoveUserFromCourseAsync(courseUserDeleteRequest);
            return NoContent();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetCourseByName([FromRoute] string name)
        {
            Course? course = await courseRepository.GetByNameAsync(name);
            return Ok(course.ToResponseFromModel());
        }

        [HttpPost]
        [Route("{name}/import-users-from-excel")]
        public async Task<IActionResult> ImportUsersFromExcel([FromRoute] string name, IFormFile file)
        {
            var isHeaderValid = await ExcelValidator.ValidateCourseUsersFile(file);
            if (!isHeaderValid)
            {
                return BadRequest("Invalid file header. The file must contain 'UserName' columns.");
            }
            List<CourseUser> res = await courseRepository.ImportCourseUserFromExcelAsync(name, file);
            return Ok(res.Select(courseUser => courseUser.ToResponseFromModel()));
        }

        [HttpGet]
        [Route("{name}/export-users-from-excel")]
        public async Task<IActionResult> ExporttUsersFromExcel([FromRoute] string name)
        {
            byte[] excelData = await courseRepository.ExportCourseUserFromExcelAsync(name);
            var fileName = $"{name}-Users-List.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileDownloadName: fileName);
        }



        [ValidateModelState]
        [HttpGet]
        [Route("{name}/users")]
        public async Task<IActionResult> GetUsersOfCourse([FromRoute] string name)
        {
            List<User>? users = await courseRepository.GetUsersOfCourseAsync(name, null);
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }



        [HttpGet]
        [Route("{name}/students")]
        public async Task<IActionResult> GetStudentsOfCourse([FromRoute] string name)
        {
            var users = await courseRepository.GetUsersOfCourseAsync(name, "Student");
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }
        [HttpGet]
        [Route("{name}/teachers")]
        public async Task<IActionResult> GetTeachersOfCourse([FromRoute] string name)
        {
            var users = await courseRepository.GetUsersOfCourseAsync(name, "Teacher");
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }

        [HttpGet]
        [Route("{name}/scores")]
        public async Task<IActionResult> GetScoresOfCourses([FromRoute] string name)
        {
            var scores = await courseRepository.GetScoresOfCourseAsync(name);
            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }
        [HttpPost]
        [Route("{name}/scores/import-from-excel")]
        [ValidateExcelFile("file")]
        public async Task<IActionResult> ImportFromExcel([FromRoute] string name, IFormFile file)
        {
            var isHeaderValid = await ExcelValidator.ValidateScoresFile(file);
            if (!isHeaderValid)
            {
                return BadRequest("Invalid file header. The file must contain 'UserName', 'Value' columns.");
            }
            List<Score> scores = await courseRepository.ImportScoresFromExcelAsync(name, file);

            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }
        [HttpPost]
        [Route("{name}/scores/export-to-excel")]
        public async Task<IActionResult> ExportToExcel([FromRoute] string name)
        {
            byte[] excelData = await courseRepository.ExportScoresToExcelAsync(name);
            var fileName = $"{name}-Scores.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelData, contentType, fileDownloadName: fileName);
        }
    }
}