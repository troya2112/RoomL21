using Microsoft.AspNetCore.Mvc.Rendering;
using RoomL21.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace RoomL21.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _dataContext;

        public CombosHelper(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<SelectListItem> GetComboEventTypes()
        {
            var list = _dataContext.EventTypes.Select(pt => new SelectListItem
            {
                Text = pt.Name,
                Value = $"{pt.Id}"
            })
                .OrderBy(pt => pt.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a event type...]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboUserTypes()
        {
            var list = _dataContext.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Name
            }).ToList().Where(u => u.Text == "Owner" || u.Text == "Organizer").ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an User Type...)",
                Value = "0"
            });
            return list;
        }

    }
}
