﻿using System.Collections.Generic;

namespace DeskBooker.Core.Domain
{
    public class DeskBooking : DeskBookingBase
    {
        public int DeskId { get; set; }
        public int? Id { get; set; }
    }
}