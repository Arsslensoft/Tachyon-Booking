using System;

namespace Tachyon.Booking.Time
{
    public struct TimeInterval
    {
        public TimeSpan Start { get; set; }
        public TimeSpan Due { get; set; }
    }
}