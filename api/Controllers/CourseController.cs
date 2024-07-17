using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Filters;
using api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
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
            return Ok(courses);
        }
        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetCourseByName([FromRoute] string name) {
            var course = await courseRepository.GetByName(name);
            return Ok(course);
        }
    }
}