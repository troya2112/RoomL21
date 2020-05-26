using RoomL21.Web.Data.Entities;
using RoomL21.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();

            User owner = await CheckUserAsync("1010", "Juan", "Ochoa", "jochoa.gaviria@hotmail.com", "311 743 7028", "Carrera 49", "Owner");
            User admin = await CheckUserAsync("2020", "Jonathan", "Ospina", "jonathanospinac@gmail.com", "311 392 6724", "Calle 70", "Admin");
            User organizer = await CheckUserAsync("3030", "Jose David", "Ocampo", "davidocampo101@gmail.com", "300 295 5427", "Calle 26", "Organizer");

            await CheckEventTypesAsync();

            await CheckOwnerAsync(owner);
            await CheckAdminAsync(admin);
            await CheckOrganizerAsync(organizer);

            await CheckRoomAsync();
            await CheckAgendasAsync();

        }

        private async Task CheckRoomAsync()
        {
            Owner owner = _dataContext.Owners.FirstOrDefault();
            if (!_dataContext.Rooms.Any())
            {
                AddRoom(50, "Carrera 90 B # 32", owner, "Es un lugar muy acojedor");
                AddRoom(120, "Carrera 50 D # 122", owner, "Si buscas economia, este es tu lugar");
                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddRoom(int capacity, string address, Owner owner, string remarks)
        {
            _dataContext.Rooms.Add(new Room
            {
                Capacity = capacity,
                Address = address,
                Owner = owner,
                Remarks = remarks
            });
        }

        private async Task CheckOrganizerAsync(User user)
        {
            if (!_dataContext.Organizers.Any())
            {
                _dataContext.Organizers.Add(new Organizer { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckAdminAsync(User user)
        {
            if (!_dataContext.Admins.Any())
            {
                _dataContext.Admins.Add(new Admin { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckOwnerAsync(User user)
        {
            if (!_dataContext.Owners.Any())
            {
                _dataContext.Owners.Add(new Owner { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }
        private async Task CheckEventTypesAsync()
        {
            if (!_dataContext.EventTypes.Any())
            {
                _dataContext.EventTypes.Add(new EventType { Name = "Boda" });
                _dataContext.EventTypes.Add(new EventType { Name = "Cumpleaños" });
                _dataContext.EventTypes.Add(new EventType { Name = "Despedida de soltero" });
                _dataContext.EventTypes.Add(new EventType { Name = "Baby Shower" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Owner");
            await _userHelper.CheckRoleAsync("Organizer");
            await _userHelper.CheckRoleAsync("Invited");
        }

        private async Task<User> CheckUserAsync(string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string role)
        {
            User user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckAgendasAsync()
        {
            if (!_dataContext.Agendas.Any())
            {
                DateTime initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                DateTime finalDate = initialDate.AddMonths(2);

                while (initialDate < finalDate)
                {
                    if (initialDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        DateTime finalDate2 = initialDate.AddHours(10);
                        while (initialDate < finalDate2)
                        {
                            _dataContext.Agendas.Add(new Agenda
                            {
                                Date = initialDate.ToUniversalTime(),
                                IsAvailable = true
                            });
                            initialDate = initialDate.AddMinutes(30);
                        }
                        initialDate = initialDate.AddHours(14);
                    }
                    else
                    {
                        initialDate = initialDate.AddDays(1);
                    }
                }
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
