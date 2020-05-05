using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RoomL21.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboEventTypes();
        IEnumerable<SelectListItem> GetComboUserTypes();

    }
}
