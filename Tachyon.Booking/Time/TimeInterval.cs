using System;
using System.Collections.Generic;
using System.Linq;

namespace Tachyon.Booking.Time
{
    public struct TimeInterval : IInterval<TimeSpan>, IEquatable<TimeInterval>, IComparable<TimeInterval>
    {
        public TimeSpan Start { get; }
        public TimeSpan Due { get; }
        public bool IsValid => Start < Due;
        public TimeInterval(TimeSpan start, TimeSpan due)
        {
            Start = start;
            Due = due;
            if (!IsValid) throw new ArgumentException("Due value must be greater than start", nameof(due));
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

        public static implicit operator TimeInterval((TimeSpan start, TimeSpan due) value) =>
            new TimeInterval(value.start, value.due);
        #endregion

        #region Comparison Operators
        public static bool operator ==(TimeInterval a, TimeInterval b) => a.Equals(b);
        public static bool operator !=(TimeInterval a, TimeInterval b) => !a.Equals(b);
        public static bool operator >(TimeInterval a, TimeInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(TimeInterval a, TimeInterval b) => a.CompareTo(b) == -1;
        public static bool operator ^(TimeInterval a, TimeInterval b) => (a.Start >= b.Start && a.Due <= b.Due); // a in b 
        public static bool operator |(TimeInterval a, TimeInterval b) => (a.Start >= b.Start && a.Start <= b.Due) || (a.Due >= b.Start && a.Due <= b.Due) || (b ^ a); // a intersects with b
        #endregion

        #region Simple Arithmetic Operators
        /// <summary>
        /// Cartesian product of two <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>The common date <see cref="TimeInterval"/> interval between two <see cref="TimeInterval"/> intervals.</returns>
        public static TimeInterval? operator &(TimeInterval a, TimeInterval b)
        {
            if (a == b)
                return a; // If any set is the same time, then by default there must be some overlap. 

            if (a.Start < b.Start) // a before b
            {
                if (a.Due > b.Start && a.Due < b.Due) // a ends after b start
                    return new TimeInterval(b.Start, a.Due);

                if (a.Due >= b.Due) // a ends after b end  (b in a)
                    return b;
            }
            else // a after b
            {
                if (b.Due > a.Start && b.Due < a.Due) // b ends after a start && b ends before a end 
                    return new TimeInterval(a.Start, b.Due);

                if (b.Due >= a.Due) // a is in b
                    return a;
            }
            return null;
        }

        /// <summary>
        /// Returns a set which includes the intervals that do not overlap with the given interval.
        /// !R = {Rmin, Rmax}, Rmin=(<see cref="TimeSpan.MinValue"/>, <see cref="a"/>.Start), Rmax=(<see cref="a"/>.Due, <see cref="TimeSpan.MaxValue"/>)
        /// </summary>
        /// <param name="a">Desired date interval to remove</param>
        /// <returns>intervals within [midnight(start)-endOfDay(end)] except a</returns>
        public static IEnumerable<TimeInterval> operator !(TimeInterval a)
        {
            yield return new TimeInterval(TimeSpan.MinValue, a.Start);
            yield return new TimeInterval(a.Due, TimeSpan.MaxValue);
        }

        /// <summary>
        /// Removes a and b intersections.
        /// A - B = !B & A
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>(All intervals except b) & a</returns>
        public static IEnumerable<TimeInterval?> operator -(TimeInterval a, TimeInterval b)
            => (!b)?.Select(bEntry => a & bEntry).Where(r => r != null);

        /// <summary>
        /// Applies a union operation between two <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>a U b</returns>
        public static IEnumerable<TimeInterval> operator +(TimeInterval a, TimeInterval b)
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
                yield return new TimeInterval(first.Start, last.Due);
            }
        }

        #endregion

        #region Collection Airthmetic Operations
        /// <summary>
        /// Cartesian product of two or more <see cref="TimeInterval"/> intervals.
        /// a & b = U(a & bi) where bi is part of b.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right intervals</param>
        /// <returns>The common date <see cref="TimeInterval"/> interval between two or more <see cref="TimeInterval"/> intervals.</returns>
        public static IEnumerable<TimeInterval?> operator &(TimeInterval a, IEnumerable<TimeInterval> b)
              => b?.Select(bEntry => a & bEntry).Where(r => r != null).Distinct();

        /// <summary>
        /// Cartesian product of two or more <see cref="TimeInterval"/> intervals.
        /// a & b = U(a & bi) where bi is part of b.
        /// </summary>
        /// <param name="a">The right interval</param>
        /// <param name="b">The left intervals</param>
        /// <returns>The common date <see cref="TimeInterval"/> interval between two or more <see cref="TimeInterval"/> intervals.</returns>
        public static IEnumerable<TimeInterval?> operator &(IEnumerable<TimeInterval> b, TimeInterval a)
            => b?.Select(bEntry => a & bEntry).Where(r => r != null).Distinct();
        /// <summary>
        /// Removes an interval from a list of <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="intervals">The initial intervals.</param>
        /// <param name="mask">The mask to apply.</param>
        /// <returns>a - mask = U(ai - mask) where ai is part of a.</returns>
        public static IEnumerable<TimeInterval> operator -(IEnumerable<TimeInterval> intervals,
                TimeInterval mask)
        {
            if (intervals == null)
            {
                yield return mask;
                yield break;
            }
            IEnumerable<TimeInterval> excludedIntervals = new List<TimeInterval>();
            foreach (var TimeInterval in intervals.Aggregate(excludedIntervals, (current, interval) => current.Union((interval - mask).Where(x => x != null)
                .Cast<TimeInterval>())))
                yield return TimeInterval;
        }

        /// <summary>
        /// Removal operator, removes date intervals from a specific date interval r
        /// </summary>
        /// <param name="r">base date interval</param>
        /// <param name="b">date intervals to be removed</param>
        /// <returns>r split into pieces after stripping b intervals from it</returns>
        public static IEnumerable<TimeInterval> operator -(TimeInterval r, IEnumerable<TimeInterval> b)
        {
            if (b == null)
            {
                yield return r;
                yield break;
            }

            IEnumerable<TimeInterval> excludedIntervals = new List<TimeInterval>();
            foreach (var TimeInterval in b.Aggregate(excludedIntervals, (current, interval) =>
                current.Union((interval - r).Where(x => x != null)
                    .Cast<TimeInterval>())))
                yield return TimeInterval;

        }

        /// <summary>
        /// Applies a union operation between two or more <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right intervals</param>
        /// <returns>a U bi where bi is part of b</returns>
        public static IEnumerable<TimeInterval> operator +(TimeInterval a, IEnumerable<TimeInterval> b)
        {
            if (b == null) return null;
            IEnumerable<TimeInterval> excludedIntervals = new List<TimeInterval>();
            return b.Aggregate(excludedIntervals, (current, interval) => current.Union(interval + a)).Distinct();
        }
        /// <summary>
        /// Applies a union operation between two or more <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="a">The right interval</param>
        /// <param name="b">The left intervals</param>
        /// <returns>a U bi where bi is part of b</returns>
        public static IEnumerable<TimeInterval> operator +(IEnumerable<TimeInterval> b, TimeInterval a)
        {
            if (b == null) return null;
            IEnumerable<TimeInterval> excludedIntervals = new List<TimeInterval>();
            return b.Aggregate(excludedIntervals, (current, interval) => current.Union(interval + a)).Distinct();
        }
        #endregion
    }
}