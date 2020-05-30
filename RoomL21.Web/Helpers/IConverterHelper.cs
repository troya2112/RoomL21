using RoomL21.Common.Models;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Web.Helpers
{
    public interface IConverterHelper
    {
        EventResponse ToEventResponse(Event eventVar);
        Task<Room> ToRoomAsync(RoomViewModel model, string path, bool isNew);
        RoomViewModel ToRoomViewModel(Room room);
        Task<Event> ToEventAsync(EventViewModel model, bool isNew);
        EventViewModel ToEventViewModel(Event Event);
    }
}
