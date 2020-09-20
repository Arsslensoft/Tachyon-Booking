using System;

namespace Tachyon.Booking.Time
{
    public struct TimeInterval : IInterval<TimeSpan>, IEquatable<TimeInterval>, IComparable<TimeInterval>
    {
        public TimeSpan Start { get; }
        public TimeSpan Due { get; }

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
        public override string ToString()
        {
            return $"{Start} -> {Due}";
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() + Due.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && this.Equals((TimeInterval)obj);
        }

        #region Conversion Operators
        public static implicit operator TimeInterval(Tuple<TimeSpan, TimeSpan> value) =>
            new TimeInterval(value.Item1, value.Item2);

        #endregion

        #region Comparison Operators
        public static bool operator ==(TimeInterval a, TimeInterval b) => a.Equals(b);
        public static bool operator !=(TimeInterval a, TimeInterval b) => !a.Equals(b);
        public static bool operator >(TimeInterval a, TimeInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(TimeInterval a, TimeInterval b) => a.CompareTo(b) == -1;
        #endregion
    }
}