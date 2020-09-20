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

        #endregion

        #region Comparison Operators
        public static bool operator ==(TimeInterval a, TimeInterval b) => a.Equals(b);
        public static bool operator !=(TimeInterval a, TimeInterval b) => !a.Equals(b);
        public static bool operator >(TimeInterval a, TimeInterval b) => a.CompareTo(b) == 1;
        public static bool operator <(TimeInterval a, TimeInterval b) => a.CompareTo(b) == -1;
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
            if (a.Start == a.Due || b.Start == b.Due)
                return null; // No actual date range

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
            => (!b).Select(bEntry => a & bEntry).Where(r => r != null);

        /// <summary>
        /// Applies a union operation between two <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="a">The left interval</param>
        /// <param name="b">The right interval</param>
        /// <returns>a U b</returns>
        public static IEnumerable<TimeInterval> operator +(TimeInterval a, TimeInterval b)
        {
            var r = a & b;
            var first = a.Start < b.Start ? a : b;
            var last = a.Start > b.Start ? a : b;
            if (r != null)
            {
                // a in b or b in a
                if (r == a) // b is bigger than a
                {
                    yield return b;
                }
                else if (r == b) // a is bigger than b
                {
                    yield return a;
                }
                else if (first.Start <= r.Value.Start || last.Start <= r.Value.Start) // left or left
                    yield return new TimeInterval(first.Start, last.Due);
            }
            else
            {
                yield return a;
                yield return b;
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
            => b.Select(bEntry => a & bEntry).Where(r => r != null);

        /// <summary>
        /// Removes an interval from a list of <see cref="TimeInterval"/> intervals.
        /// </summary>
        /// <param name="intervals">The initial intervals.</param>
        /// <param name="mask">The mask to apply.</param>
        /// <returns>a - mask = U(ai - mask) where ai is part of a.</returns>
        public static IEnumerable<TimeInterval> operator -(IEnumerable<TimeInterval> intervals,
            TimeInterval mask)
        {
            IEnumerable<TimeInterval> excludedIntervals = new List<TimeInterval>();
            return intervals.Aggregate(excludedIntervals, (current, interval) => current.Union((interval - mask).Where(x => x != null)
                .Cast<TimeInterval>()));
        }

        /// <summary>
        /// Removal operator, removes date intervals from a specific date interval r
        /// </summary>
        /// <param name="r">base date interval</param>
        /// <param name="b">date intervals to be removed</param>
        /// <returns>r split into pieces after stripping b intervals from it</returns>
        public static IEnumerable<TimeInterval> operator -(TimeInterval r, IEnumerable<TimeInterval> b)
        {
            if (r == null || b == null) yield break;
            var masks = b.OrderBy(x => x.Due).ToList();
            IEnumerable<TimeInterval> ranges = new List<TimeInterval>() { r };
            ranges = masks.Aggregate(ranges, (current, mask) => (current - mask));

            // unify intervals if possible
            var orderedIntervals = ranges.OrderBy(x => x.Start).ToList();
            for (var i = 0; i < orderedIntervals.Count - 1; i++)
            {
                var union = (orderedIntervals[i] + orderedIntervals[i + 1]).ToList();
                if (union.Count != 1) continue;
                orderedIntervals[i] = union[0];
                orderedIntervals.RemoveAt(i + 1);
            }

            foreach (var interval in orderedIntervals)
                yield return interval;

        }


        #endregion
    }
}