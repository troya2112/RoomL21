using RoomL21.Common.Models;
using RoomL21.Web.Data;
using RoomL21.Web.Data.Entities;
using RoomL21.Web.Models;
using System.Threading.Tasks;

namespace RoomL21.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public ConverterHelper(DataContext dataContext, ICombosHelper combosHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
        }

        public EventResponse ToEventResponse(Event eventVar)
        {
            if (eventVar == null)
            {
                return null;
            }

            return new EventResponse
            {
                Id = eventVar.Id,
                Name = eventVar.Name,
                EventType = eventVar.EventType.Name,
                InvitedsNumber = eventVar.InvitesNumber,
                Remarks = eventVar.Remarks
            };
        }

        public async Task<Room> ToRoomAsync(RoomViewModel model, string path, bool isNew)
        {
            var room = new Room
            {
                Id = isNew ? 0 : model.Id,
                Agendas = model.Agendas,
                Address = model.Address,
                Capacity = model.Capacity,
                ImageUrl = path,
                Events = model.Events,
                Owner = await _dataContext.Owners.FindAsync(model.OwnerId),
                Remarks = model.Remarks
            };

            return room;
        }

        public RoomViewModel ToRoomViewModel(Room room)
        {
            return new RoomViewModel
            {
                Id = room.Id,
                Agendas = room.Agendas,
                Address = room.Address,
                Capacity = room.Capacity,
                ImageUrl = room.ImageUrl,
                Events = room.Events,
                Owner = room.Owner,
                Remarks = room.Remarks,
                OwnerId = room.Owner.Id,
            };
        }

        public async Task<Event> ToEventAsync(EventViewModel model, bool isNew)
        {
            return new Event
            {
                Id = isNew ? 0 : model.Id,
                Agenda = model.Agenda,
                EventType = await _dataContext.EventTypes.FindAsync(model.EventTypeId),
                Room = await _dataContext.Rooms.FindAsync(model.RoomId),
                Remarks = model.Remarks,
                Inviteds = model.Inviteds,
                InvitesNumber = model.InvitesNumber,
                Name = model.Name,
                Organizer = model.Organizer
            };
        }

        public EventViewModel ToEventViewModel(Event Event)
        {
            return new EventViewModel
            {
                Id = Event.Id,
                Date = Event.Agenda.Date,
                RoomId = Event.Room.Id,
                Room = Event.Room,
                EventType = Event.EventType,
                Agenda = Event.Agenda,
                Inviteds = Event.Inviteds,
                Organizer = Event.Organizer,
                EventTypeId = Event.EventType.Id,
                InvitesNumber = Event.InvitesNumber,
                Name = Event.Name,
                Remarks = Event.Remarks,
                EventTypes = _combosHelper.GetComboEventTypes()
            };
        }
    }
}
