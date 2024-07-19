using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Requests;
using api.Models;

namespace api.Interfaces
{
    public interface IScoreRepository
    {
        public  Task<List<Score>> GetAllAsync();
        public Task<Score> CreateOrUpdateAsync(ScoreRequest scoreRequest);
        public Task RemoveAsync(ScoreDeleteRequest scoreDeleteRequest);
    }
}