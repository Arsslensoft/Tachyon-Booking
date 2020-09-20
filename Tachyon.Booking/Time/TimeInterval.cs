using System;

namespace Tachyon.Booking.Time
{
    public struct TimeInterval : IEquatable<TimeInterval>, IComparable<TimeInterval>
    {
        public TimeSpan Start { get; set; }
        public TimeSpan Due { get; set; }

        public TimeInterval(TimeSpan start, TimeSpan due)
        {
            Start = start;
            Due = due;
        }

        public bool Equals(TimeInterval other)
        {
            return other.Start == Start && other.Due == Due;
        }

        public int CompareTo(TimeInterval other)
        {
            var startResult = Start.CompareTo(other.Start);
            return startResult == 0 ? Due.CompareTo(other.Due) : startResult;
        }
    }
}