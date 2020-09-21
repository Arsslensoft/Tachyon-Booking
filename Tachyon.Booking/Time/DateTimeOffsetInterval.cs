using System;
using System.Collections.Generic;
using System.Linq;
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

        public static implicit operator DateTimeOffsetInterval((DateTimeOffset start, DateTimeOffset due) value) =>
            new DateTimeOffsetInterval(value.start, value.due);
        #endregion

        #region Comparison Operators
        public static bool operator ==(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.Equals(b);
        public static bool operator !=(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => !a.Equals(b);
        public static bool operator >(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => a.CompareTo(b) == -1;
        public static bool operator ^(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => (a.Start >= b.Start && a.Due <= b.Due); // a in b 
        public static bool operator |(DateTimeOffsetInterval a, DateTimeOffsetInterval b) => (a.Start >= b.Start && a.Start <= b.Due) || (a.Due >= b.Start && a.Due <= b.Due) || (b ^ a); // a intersects with b
        #endregion

        #region Simple Arithmetic Operators
        /// <summary>
        /// Cartesian product of two <see cref="DateTimeOffsetInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>The common date <see cref="DateTimeOffsetInterval"/> interval between two <see cref="DateTimeOffsetInterval"/> intervals.</returns>
        public static DateTimeOffsetInterval? operator &(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
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

        /// <summary>
        /// Returns a set which includes the intervals that do not overlap with the given interval.
        /// !R = {Rmin, Rmax}, Rmin=(<see cref="DateTimeOffset.MinValue"/>, <see cref="a"/>.Start), Rmax=(<see cref="a"/>.Due, <see cref="DateTimeOffset.MaxValue"/>)
        /// </summary>
        /// <param name="a">Desired date interval to remove</param>
        /// <returns>intervals within [midnight(start)-endOfDay(end)] except a</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator !(DateTimeOffsetInterval a)
        {
            yield return new DateTimeOffsetInterval(DateTimeOffset.MinValue, a.Start);
            yield return new DateTimeOffsetInterval(a.Due, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Removes a and b intersections.
        /// A - B = !B & A
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>(All intervals except b) & a</returns>
        public static IEnumerable<DateTimeOffsetInterval?> operator -(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
            => (!b)?.Select(bEntry => a & bEntry).Where(r => r != null);

        /// <summary>
        /// Applies a union operation between two <see cref="DateTimeOffsetInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>a U b</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator +(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {

            if (a.Start > b.Due || a.Due < b.Start)    // a inter b = empty
            {
                yield return a;
                yield return b;
            }
            else if (a ^ b) // a in b
                yield return b;
            else if (b ^ a) // b in a
                yield return a;
            else
            {
                var first = a.Start < b.Start ? a : b;
                var last = a.Start > b.Start ? a : b;
                yield return new DateTimeOffsetInterval(first.Start, last.Due);
            }
        }

        #endregion

        #region Collection Airthmetic Operations
        /// <summary>
        /// Cartesian product of two or more <see cref="DateTimeOffsetInterval"/> intervals.
        /// a & b = U(a & bi) where bi is part of b.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right intervals</param>
        /// <returns>The common date <see cref="DateTimeOffsetInterval"/> interval between two or more <see cref="DateTimeOffsetInterval"/> intervals.</returns>
        public static IEnumerable<DateTimeOffsetInterval?> operator &(DateTimeOffsetInterval a, IEnumerable<DateTimeOffsetInterval> b)
              => b?.Select(bEntry => a & bEntry).Where(r => r != null).Distinct();

        /// <summary>
        /// Cartesian product of two or more <see cref="DateTimeOffsetInterval"/> intervals.
        /// a & b = U(a & bi) where bi is part of b.
        /// </summary>
        /// <param name="a">The right interval</param>
        /// <param name="b">The left intervals</param>
        /// <returns>The common date <see cref="DateTimeOffsetInterval"/> interval between two or more <see cref="DateTimeOffsetInterval"/> intervals.</returns>
        public static IEnumerable<DateTimeOffsetInterval?> operator &(IEnumerable<DateTimeOffsetInterval> b, DateTimeOffsetInterval a)
            => b?.Select(bEntry => a & bEntry).Where(r => r != null).Distinct();
        /// <summary>
        /// Removes an interval from a list of <see cref="DateTimeOffsetInterval"/> intervals.
        /// </summary>
        /// <param name="intervals">The initial intervals.</param>
        /// <param name="mask">The mask to apply.</param>
        /// <returns>a - mask = U(ai - mask) where ai is part of a.</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator -(IEnumerable<DateTimeOffsetInterval> intervals,
                DateTimeOffsetInterval mask)
        {
            if (intervals == null)
            {
                yield return mask;
                yield break;
            }
            IEnumerable<DateTimeOffsetInterval> excludedIntervals = new List<DateTimeOffsetInterval>();
            foreach (var dateTimeOffsetInterval in intervals.Aggregate(excludedIntervals, (current, interval) => current.Union((interval - mask).Where(x => x != null)
                .Cast<DateTimeOffsetInterval>())))
                yield return dateTimeOffsetInterval;
        }

        /// <summary>
        /// Removal operator, removes date intervals from a specific date interval r
        /// </summary>
        /// <param name="r">base date interval</param>
        /// <param name="b">date intervals to be removed</param>
        /// <returns>r split into pieces after stripping b intervals from it</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator -(DateTimeOffsetInterval r, IEnumerable<DateTimeOffsetInterval> b)
        {
            if (b == null)
            {
                yield return r;
                yield break;
            }

            IEnumerable<DateTimeOffsetInterval> excludedIntervals = new List<DateTimeOffsetInterval>();
            foreach (var dateTimeOffsetInterval in b.Aggregate(excludedIntervals, (current, interval) =>
                current.Union((interval - r).Where(x => x != null)
                    .Cast<DateTimeOffsetInterval>())))
                yield return dateTimeOffsetInterval;

        }

        /// <summary>
        /// Applies a union operation between two or more <see cref="DateTimeOffsetInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right intervals</param>
        /// <returns>a U bi where bi is part of b</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator +(DateTimeOffsetInterval a, IEnumerable<DateTimeOffsetInterval> b)
        {
            if (b == null) return null;
            IEnumerable<DateTimeOffsetInterval> excludedIntervals = new List<DateTimeOffsetInterval>();
            return b.Aggregate(excludedIntervals, (current, interval) => current.Union(interval + a)).Distinct();
        }
        /// <summary>
        /// Applies a union operation between two or more <see cref="DateTimeOffsetInterval"/> intervals.
        /// </summary>
        /// <param name="a">The right interval</param>
        /// <param name="b">The left intervals</param>
        /// <returns>a U bi where bi is part of b</returns>
        public static IEnumerable<DateTimeOffsetInterval> operator +(IEnumerable<DateTimeOffsetInterval> b, DateTimeOffsetInterval a)
        {
            if (b == null) return null;
            IEnumerable<DateTimeOffsetInterval> excludedIntervals = new List<DateTimeOffsetInterval>();
            return b.Aggregate(excludedIntervals, (current, interval) => current.Union(interval + a)).Distinct();
        }
        #endregion
    }

}
