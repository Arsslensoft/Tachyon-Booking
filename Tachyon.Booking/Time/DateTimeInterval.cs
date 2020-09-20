using System;

namespace Tachyon.Booking.Time
{
    public struct DateTimeInterval : IInterval<DateTime>, IEquatable<DateTimeInterval>, IComparable<DateTimeInterval>
    {
        public DateTime Start { get; }
        public DateTime Due { get; }

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
            return obj != null && this.Equals((DateTimeInterval)obj);
        }

        #region Conversion Operators
        public static implicit operator DateTimeInterval(Tuple<DateTime, DateTime> value) =>
            new DateTimeInterval(value.Item1, value.Item2);

        #endregion

        #region Comparison Operators
        public static bool operator ==(DateTimeInterval a, DateTimeInterval b) => a.Equals(b);
        public static bool operator !=(DateTimeInterval a, DateTimeInterval b) => !a.Equals(b);
        public static bool operator >(DateTimeInterval a, DateTimeInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(DateTimeInterval a, DateTimeInterval b) => a.CompareTo(b) == -1;
        #endregion
    }
}