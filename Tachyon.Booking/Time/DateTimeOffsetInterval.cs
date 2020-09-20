using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Time
{
    public struct DateTimeOffsetInterval : IInterval<DateTimeOffset>, IEquatable<DateTimeOffsetInterval>, IComparable<DateTimeOffsetInterval>
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset Due { get; }
        public bool IsValid => Start < Due;
        public DateTimeOffsetInterval(DateTimeOffset start, DateTimeOffset due)
        {
            Start = start;
            Due = due;
            if (!IsValid) throw new ArgumentException("Due value must be greater than start", nameof(due));
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

        #region Arithmetic Operators
        /// <summary>
        /// Cartesian product of two <see cref="DateTimeOffsetInterval"/> intervals
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns></returns>
        public static DateTimeOffsetInterval? operator &(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            if (a.Start == a.Due || b.Start == b.Due)
                return null; // No actual date range

            if (a == b)
                return a; // If any set is the same time, then by default there must be some overlap. 

            if (a.Start < b.Start) // a before b
            {
                if (a.Due > b.Start && a.Due < b.Due) // a ends after b start
                    return new DateTimeOffsetInterval(b.Start, a.Due);

                if (a.Due >= b.Due) // a ends after b end  (b in a)
                    return b;
            }
            else // a after b
            {
                if (b.Due > a.Start && b.Due < a.Due) // b ends after a start && b ends before a end 
                    return new DateTimeOffsetInterval(a.Start, b.Due);

                if (b.Due >= a.Due) // a is in b
                    return a;
            }
            return null;
        }

        #endregion
    }

}
