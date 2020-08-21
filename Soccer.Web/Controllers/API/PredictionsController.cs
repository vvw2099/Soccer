using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Common.Models;
using System.Globalization;
using Soccer.Web.Resources;

namespace Soccer.Web.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController:ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public PredictionsController(DataContext dataContext,IConverterHelper converterHelper, IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<UserEntity> users = await _dataContext.Users
                .Include(u => u.Team)
                .Include(u => u.Predictions)
                .ToListAsync();

            List<PositionResponse> positionResponses = users.Select(u => new PositionResponse {
                Points = u.Points,
                UserResponse = _converterHelper.ToUserResponse(u)
            }).ToList();

            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (var item in list)
            {
                item.Ranking = i;
                i++;
            }
            return Ok(list);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetPositionsByTournament([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TournamentEntity tournament = await _dataContext.Tournaments
                 .Include(t => t.Groups)
                .ThenInclude(g => g.Matches)
                .ThenInclude(m => m.Predictions)
                .ThenInclude(p => p.User)
                .ThenInclude(u => u.Team)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tournament == null)
            {
                return BadRequest("Tournament doesn't exists.");
            }
            List<PositionResponse> positionResponses = new List<PositionResponse>();

            foreach (GroupEntity groupEntity in tournament.Groups)
            {
                foreach (MatchEntity matchEntity in groupEntity.Matches)
                {
                    foreach(PredictionEntity predictionEntity in matchEntity.Predictions)
                    {
                        PositionResponse positionResponse = positionResponses
                            .FirstOrDefault(pr => pr.UserResponse.Id == predictionEntity.User.Id);

                        if (positionResponse == null)
                        {
                            positionResponses.Add(new PositionResponse
                            {
                                Points = predictionEntity.Points,
                                UserResponse = _converterHelper.ToUserResponse(predictionEntity.User),
                            });
                        }
                        else
                        {
                            positionResponse.Points = predictionEntity.Points;
                        }
                    }
                }
            }
            List<PositionResponse> list = positionResponses.OrderByDescending(pr => pr.Points).ToList();
            int i = 1;
            foreach (PositionResponse item in list)
            {
                item.Ranking = i;
                i++;
            }

            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> PostPrediction([FromBody]PredictionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            MatchEntity matchEntity = await _dataContext.Matches.FindAsync(request.MatchId);
            if (matchEntity == null)
            {
                return BadRequest(Resource.MatchDoesntExists);
            }
            if (matchEntity.IsClosed)
            {
                return BadRequest(Resource.MatchAlreadyClosed);
            }

            UserEntity userEntity = await _userHelper.GetUserAsync(request.UserId);
            if (userEntity == null)
            {
                return BadRequest(Resource.UserDoesntExists);
            }
            if(matchEntity.Date <= DateTime.UtcNow)
            {
                return BadRequest(Resource.MatchAlreadyStarts);
            }

            PredictionEntity predictionEntity = await _dataContext.Predictions
                .FirstOrDefaultAsync(p=> p.User.Id == request.UserId.ToString() && p.Match.Id==request.MatchId);
            if (predictionEntity == null)
            {
                predictionEntity = new PredictionEntity
                {
                    GoalsLocal = request.GoalsLocal,
                    GoalsVisitor = request.GoalsVisitor,
                    Match = matchEntity,
                    User = userEntity
                };

                _dataContext.Predictions.Add(predictionEntity);
            }
            else
            {
                predictionEntity.GoalsLocal = request.GoalsLocal;
                predictionEntity.GoalsVisitor = request.GoalsVisitor;
                _dataContext.Predictions.Update(predictionEntity);
            }

            await _dataContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost]
        [Route("GetPredictionsForUser")]
        public async Task<IActionResult> GetPredictionsForUser([FromBody] PredictionsForUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CultureInfo cultureInfo = new CultureInfo(request.CultureInfo);
            Resource.Culture = cultureInfo;

            TournamentEntity tournament = await _dataContext.Tournaments.FindAsync(request.TournamentId);
            if (tournament == null)
            {
                return BadRequest(Resource.TournamentDoesntExists);
            }

            UserEntity userEntity= await _dataContext.Users
                .Include(u => u.Team)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Local)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(m => m.Visitor)
                .Include(u => u.Predictions)
                .ThenInclude(p => p.Match)
                .ThenInclude(p => p.Group)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(u => u.Id == request.UserId.ToString());
            if (userEntity == null)
            {
                return BadRequest(Resource.UserDoesntExists);
            }

            // Add precitions already done
            List<PredictionResponse> predictionResponses = new List<PredictionResponse>();
            foreach (PredictionEntity predictionEntity in userEntity.Predictions)
            {
                if (predictionEntity.Match.Group.Tournament.Id == request.TournamentId)
                {
                    predictionResponses.Add(_converterHelper.ToPredictionResponse(predictionEntity));
                }
            }

            // Add precitions undone
            List<MatchEntity> matches = await _dataContext.Matches
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .Where(m => m.Group.Tournament.Id == request.TournamentId)
                .ToListAsync();
            foreach (MatchEntity matchEntity in matches)
            {
                PredictionResponse predictionResponse = predictionResponses.FirstOrDefault(pr => pr.Match.Id == matchEntity.Id);
                if (predictionResponse == null)
                {
                    predictionResponses.Add(new PredictionResponse
                    {
                        Match = _converterHelper.ToMatchResponse(matchEntity),
                    });
                }
            }

            return Ok(predictionResponses.OrderBy(pr => pr.Id).ThenBy(pr => pr.Match.Date));
        } 
    }
}
