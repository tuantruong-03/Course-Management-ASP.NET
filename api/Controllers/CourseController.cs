using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.Filters;
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
    [Authorize]
    public class CourseController : ControllerBase
    {
         private readonly ICourseRepository courseRepository;
        public CourseController(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var courses = await courseRepository.GetAllAsync();
            return Ok(courses.Select(course => course.ToResponseFromModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseCreateRequest courseCreateRequest){
            var course = await courseRepository.CreateAsync(courseCreateRequest);
            return Ok(course.ToResponseFromModel());

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseCreateRequest courseCreateRequest){
            Course? course = await courseRepository.UpdateAsync(id, courseCreateRequest);
            return Ok(course.ToResponseFromModel());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            await courseRepository.RemoveAsync(id);
            return NoContent();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetCourseByName([FromRoute] string name) {
            Course? course = await courseRepository.GetByNameAsync(name);
            return Ok(course.ToResponseFromModel());
        }

        [ValidateModelState]
        [HttpPost]
        [Route("users")]
         public async Task<IActionResult> AddUserToCourse([FromBody] CourseUserCreateRequest courseUserCreateRequest) {
            CourseUser? courseUser = await courseRepository.AddUserToCourseAsync(courseUserCreateRequest);
            return Ok(courseUser.ToResponseFromModel());
        }

        [ValidateModelState]
        [HttpDelete]
        [Route("users")]
         public async Task<IActionResult> RemoveUserFromCourse([FromBody] CourseUserDeleteRequest courseUserDeleteRequest) {
            await courseRepository.RemoveUserFromCourseAsync(courseUserDeleteRequest);
            return NoContent();
        }

        [ValidateModelState]
        [HttpGet]
        [Route("{name}/users")]
        public async Task<IActionResult> GetUsersOfCourse([FromRoute] string name) {
            List<User>? users = await courseRepository.GetUsersOfCourseAsync(name, null);
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }

       

        [HttpGet]
        [Route("{name}/students")]
        public async Task<IActionResult> GetStudentsOfCourse([FromRoute] string name) {
            var users = await courseRepository.GetUsersOfCourseAsync(name, "Student");
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }
        [HttpGet]
        [Route("{name}/teachers")]
        public async Task<IActionResult> GetTeachersOfCourse([FromRoute] string name) {
            var users = await courseRepository.GetUsersOfCourseAsync(name, "Teacher");
            return Ok(users.Select(user => user.ToResponseFromModel()));
        }

        [HttpGet]
        [Route("{name}/scores")]
        public async Task<IActionResult> GetScoresOfCourses([FromRoute] string name) {
            var scores = await courseRepository.GetScoresOfCourseAsync(name);
            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }
    }
}