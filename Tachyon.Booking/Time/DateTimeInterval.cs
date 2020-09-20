using System;

namespace Tachyon.Booking.Time
{
    public struct DateTimeInterval : IEquatable<DateTimeInterval>, IComparable<DateTimeInterval>
    {
        public DateTime Start { get; set; }
        public DateTime Due { get; set; }

        public DateTimeInterval(DateTime start, DateTime due)
        {
            Start = start;
            Due = due;
        }

        public bool Equals(DateTimeInterval other)
        {
            return other.Start == Start && other.Due == Due;
        }

        public int CompareTo(DateTimeInterval other)
        {
            var startResult = Start.CompareTo(other.Start);
            return startResult == 0 ? Due.CompareTo(other.Due) : startResult;
        }
    }
}