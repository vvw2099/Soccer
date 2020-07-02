using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _dataContext;

        public CombosHelper(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IEnumerable<SelectListItem> GetComboTeams()
        {
            List<SelectListItem> list = _dataContext.Teams.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a team...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboTeams(int id)
        {
            List<SelectListItem> list = _dataContext.GroupDetails
                .Include(gd => gd.Team)
                .Where(gd => gd.Group.Id == id)
                .Select(gd => new SelectListItem
                {
                    Text = gd.Team.Name,
                    Value = $"{gd.Team.Id}"
                })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Team...]",
                Value = "0"
            });

            return list;
        }
    }
}
