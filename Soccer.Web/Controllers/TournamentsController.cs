using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Soccer.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TournamentsController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMatchHelper _matchHelper;

        public TournamentsController(DataContext dataContext, IConverterHelper converterHelper, 
            ICombosHelper combosHelper,IMatchHelper matchHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
            _combosHelper = combosHelper;
            _matchHelper = matchHelper;
        }
        public async Task<IActionResult> CloseMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MatchEntity matchEntity= await _dataContext.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchEntity == null)
            {
                return NotFound();
            }
            CloseMatchViewModel model = new CloseMatchViewModel
            {
                Group = matchEntity.Group,
                GroupId = matchEntity.Group.Id,
                Local = matchEntity.Local,
                LocalId = matchEntity.Local.Id,
                MatchId = matchEntity.Id,
                Visitor = matchEntity.Visitor,
                VisitorId = matchEntity.Visitor.Id
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseMatch(CloseMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _matchHelper.CloseMatchAsync(model.MatchId, model.GoalsLocal.Value, model.GoalsLocal.Value);
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            model.Group = await _dataContext.Groups.FindAsync(model.GroupId);
            model.Local = await _dataContext.Teams.FindAsync(model.LocalId);
            model.Visitor = await _dataContext.Teams.FindAsync(model.VisitorId);

            return View(model);
        }

        public async Task<IActionResult> Index()
        {

            return View(await _dataContext.Tournaments
                .Include(g=> g.Groups)
                .OrderBy(t=> t.StartDate)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = string.Empty;

                if(model.LogoFile != null)
                {
                    path = $"~/images/Tournaments/{model.LogoFile}.jpg";
                }

                TournamentEntity tournament = _converterHelper.ToTournamentEntity(model, path, true);
                _dataContext.Add(tournament);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournament = await _dataContext.Tournaments.FindAsync(id);

            if(tournament == null)
            {
                return NotFound();
            }

            TournamentViewModel model = _converterHelper.ToTournamentViewModel(tournament);
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(TournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.LogoPath;

                if(model.LogoFile != null)
                {
                    path = model.LogoFile.ToString();
                }

                TournamentEntity tournamentEntity = _converterHelper.ToTournamentEntity(model, path, false);
                _dataContext.Update(tournamentEntity);
                await _dataContext.SaveChangesAsync();

                RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournamentEntity = await _dataContext.Tournaments
                .FirstOrDefaultAsync(m => m.Id == id);

            if(tournamentEntity == null)
            {
                return NotFound();
            }

            _dataContext.Remove(tournamentEntity);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournamentEntity = await _dataContext.Tournaments
               .Include(t => t.Groups)
               .ThenInclude(t => t.Matches)
               .ThenInclude(t => t.Local)
               .Include(t => t.Groups)
               .ThenInclude(t => t.Matches)
               .ThenInclude(t => t.Visitor)
               .Include(t => t.Groups)
               .ThenInclude(t => t.GroupDetails)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (tournamentEntity == null)
            {
                return NotFound();
            }

            
            return View(tournamentEntity);
        }

        public async Task<IActionResult> AddGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TournamentEntity tournamentEntity = await _dataContext.Tournaments.FindAsync(id);

            if (tournamentEntity == null)
            {
                return NotFound();
            }

            GroupViewModel model = new GroupViewModel
            {
                Tournament = tournamentEntity,
                TournamentId = tournamentEntity.Id
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupEntity groupEntity = await _converterHelper.ToGroupEntityAsync(model, true);
                _dataContext.Add(groupEntity);
                await _dataContext.SaveChangesAsync();

                return RedirectToAction($"{nameof(Details)}/{ model.TournamentId}");
            }
            return View(model);
        }
        public async Task<IActionResult> EditGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity groupEntity = await _dataContext.Groups
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            GroupViewModel model = _converterHelper.ToGroupViewModel(groupEntity);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupEntity groupEntity = await _converterHelper.ToGroupEntityAsync(model, false);
                _dataContext.Update(groupEntity);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity groupEntity = await _dataContext.Groups
                .Include(g => g.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            _dataContext.Remove(groupEntity);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction($"{nameof(Details)}/{groupEntity.Tournament.Id}");
        }
        public async Task<IActionResult> DetailsGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity groupEntity= await _dataContext.Groups
                .Include(g => g.Matches)
                .ThenInclude(g => g.Local)
                .Include(g => g.Matches)
                .ThenInclude(g => g.Visitor)
                .Include(g => g.Tournament)
                .Include(g => g.GroupDetails)
                .ThenInclude(gd => gd.Team)
                .FirstOrDefaultAsync(g => g.Id == id);

            if(groupEntity== null)
            {
                return NotFound();
            }


            return View(groupEntity);
        }

        public async Task<IActionResult> AddGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupEntity groupEntity = await _dataContext.Groups.FindAsync(id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            GroupDetailViewModel model = new GroupDetailViewModel
            {
                Group = groupEntity,
                GroupId = groupEntity.Id,
                Teams = _combosHelper.GetComboTeams()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupDetailEntity groupDetailEntity = await _converterHelper.ToGroupDetailEntityAsync(model, true);
                _dataContext.Add(groupDetailEntity);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }

            model.Group = await _dataContext.Groups.FindAsync(model.GroupId);
            model.Teams = _combosHelper.GetComboTeams();

            return View(model);
        }
        public async Task<IActionResult> EditGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupDetailEntity groupDetailEntity=await _dataContext.GroupDetails
                 .Include(gd => gd.Group)
                .Include(gd => gd.Team)
                .FirstOrDefaultAsync(gd => gd.Id == id);

            if (groupDetailEntity == null)
            {
                return NotFound();
            }

            GroupDetailViewModel model = _converterHelper.ToGroupDetailViewModel(groupDetailEntity);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroupDetail(GroupDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                GroupDetailEntity groupDetailEntity = await _converterHelper.ToGroupDetailEntityAsync(model, false);
                _dataContext.Update(groupDetailEntity);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteGroupDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GroupDetailEntity groupDetailEntity = await _dataContext.GroupDetails
                .Include(gd => gd.Group)
                .FirstOrDefaultAsync(gd => gd.Id == id);

            if (groupDetailEntity==null)
            {
                return NotFound();
            }

            _dataContext.Remove(groupDetailEntity);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsGroup)}/{groupDetailEntity.Group.Id}");
        }
        public async Task<IActionResult> AddMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GroupEntity groupEntity = await _dataContext.Groups.FindAsync(id);

            if (groupEntity == null)
            {
                return NotFound();
            }

            MatchViewModel model = new MatchViewModel
            {
                Date = DateTime.Today,
                Group = groupEntity,
                GroupId = groupEntity.Id,
                Teams = _combosHelper.GetComboTeams(groupEntity.Id)
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.LocalId != model.VisitorId)
                {
                    MatchEntity matchEntity = await _converterHelper.ToMatchEntityAsync(model, true);
                    _dataContext.Add(matchEntity);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
                }
                ModelState.AddModelError(string.Empty, "The local and visitor must be differents teams.");
            }

            model.Group = await _dataContext.Groups.FindAsync(model.GroupId);
            model.Teams =  _combosHelper.GetComboTeams(model.GroupId);

            return View(model);
        }
        public async Task<IActionResult> EditMatch(int? id)
        {
            if(id == null){
                return NotFound();
            }

            MatchEntity matchEntity=await _dataContext.Matches
                .Include(m => m.Group)
                .Include(m => m.Local)
                .Include(m => m.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matchEntity == null)
            {
                return NotFound();
            }

            MatchViewModel model = _converterHelper.ToMatchViewModel(matchEntity);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatch(MatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                MatchEntity matchEntity = await _converterHelper.ToMatchEntityAsync(model, false);
                _dataContext.Update(matchEntity);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsGroup)}/{model.GroupId}");
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteMatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MatchEntity matchEntity= await _dataContext.Matches
                 .Include(m => m.Group)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matchEntity == null)
            {
                return NotFound();
            }

            _dataContext.Remove(matchEntity);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction($"{nameof(DetailsGroup)}/{matchEntity.Group.Id}");
        }
    }
}
