using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Time
{
    public struct DateTimeOffsetInterval : IEquatable<DateTimeOffsetInterval>, IComparable<DateTimeOffsetInterval>
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset Due { get; set; }

        public DateTimeOffsetInterval(DateTimeOffset start, DateTimeOffset due)
        {
            Start = start;
            Due = due;
        }

        public bool Equals(DateTimeOffsetInterval other)
        {
            return other.Start == Start && other.Due == Due;
        }

        public int CompareTo(DateTimeOffsetInterval other)
        {
            var startResult = Start.CompareTo(other.Start);
            return startResult == 0 ? Due.CompareTo(other.Due) : startResult;
        }
    }

}
