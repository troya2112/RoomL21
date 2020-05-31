using Newtonsoft.Json;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RoomL21.Prism.Views
{
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent(); InitializeComponent();
            RoomResponse Room = JsonConvert.DeserializeObject<RoomResponse>(Settings.Room);
            if (!string.IsNullOrWhiteSpace(Room.Address))
            {
                string direcction = Room.Address;
                MyMap.Pins.Clear();
                List<Position> positionsList = new List<Position>(new Geocoder().GetPositionsForAddressAsync(direcction).Result);
                if (positionsList.Count != 0)
                {
                    var position = positionsList.FirstOrDefault<Position>();
                    Pin pin = new Pin
                    {
                        Address = direcction,
                        Label = direcction,
                        Position = position
                    };
                    MyMap.Pins.Add(pin);
                    MyMap.MoveToRegion(
                        MapSpan.FromCenterAndRadius(
                            position, Distance.FromKilometers(100)));
                }
            }
        }
    }
}
