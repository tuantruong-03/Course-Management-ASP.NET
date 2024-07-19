using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.Filters;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/scores")]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreRepository scoreRepository;
        public ScoreController(IScoreRepository scoreRepository)
        {
            this.scoreRepository = scoreRepository;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScores() {
            List<Score> scores = await scoreRepository.GetAllAsync();
            return Ok(scores.Select(score => score.ToResponseFromModel()));
        }

        [ValidateModelState]
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ScoreRequest scoreRequest) {
            Score score = await scoreRepository.CreateOrUpdateAsync(scoreRequest);
            return Ok(score.ToResponseFromModel());
        }

        [ValidateModelState]
        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] ScoreDeleteRequest scoreDeleteRequest) {
           await scoreRepository.RemoveAsync(scoreDeleteRequest);
            return NoContent();
        }
        
    }
}