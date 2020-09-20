using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Time
{
    public struct DateTimeOffsetInterval : IInterval<DateTimeOffset>, IEquatable<DateTimeOffsetInterval>, IComparable<DateTimeOffsetInterval>
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset Due { get; }

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
            return obj != null && this.Equals((DateTimeOffsetInterval)obj);
        }

        #region Conversion Operators
        public static implicit operator DateTimeOffsetInterval(Tuple<DateTimeOffset, DateTimeOffset> value) =>
            new DateTimeOffsetInterval(value.Item1, value.Item2);

        #endregion

        #region Comparison Operators
        public static bool operator ==(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.Equals(b);
        public static bool operator !=(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => !a.Equals(b);
        public static bool operator >(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.CompareTo(b) == -1;
        #endregion
    }

}
