using RoomBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomBooking.Wpf.DataTransferObjects
{
    public class BookingDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Customer { get; set; }
    }
}
