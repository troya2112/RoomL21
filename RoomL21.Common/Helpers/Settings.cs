using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace RoomL21.Common.Helpers
{
    public static class Settings
    {
        private const string _event = "Event";
        private const string _token = "Token";
        private const string _organizer = "Organizer";
        private const string _invited = "Invited";
        private const string _isRemembered = "IsRemembered";
        private const string _room = "Room";
        private const string _eventDate = "EventDate";
        private static readonly string _stringDefault = string.Empty;
        private static readonly bool _boolDefault = false;
        private static readonly string _userType = string.Empty;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string Room
        {
            get => AppSettings.GetValueOrDefault(_room, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_room, value);
        }

        public static string EventDate
        {
            get => AppSettings.GetValueOrDefault(_eventDate, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_eventDate, value);
        }

        public static string Token
        {
            get => AppSettings.GetValueOrDefault(_token, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_token, value);
        }

        public static string Organizer
        {
            get => AppSettings.GetValueOrDefault(_organizer, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_organizer, value);
        }

        public static string Invited
        {
            get => AppSettings.GetValueOrDefault(_invited, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_invited, value);
        }

        public static string Event
        {
            get => AppSettings.GetValueOrDefault(_event, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_event, value);
        }

        public static string UserType
        {
            get => AppSettings.GetValueOrDefault(_userType, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_userType, value);
        }


        public static bool IsRemembered
        {
            get => AppSettings.GetValueOrDefault(_isRemembered, _boolDefault);
            set => AppSettings.AddOrUpdateValue(_isRemembered, value);
        }

    }
}
