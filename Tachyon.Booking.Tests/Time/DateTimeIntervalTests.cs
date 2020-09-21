using System;
using System.Collections.Generic;
using System.Linq;
using Tachyon.Booking.Time;
using Xunit;

namespace Tachyon.Booking.Tests.Time
{
    public class DateTimeIntervalTests
    {
        public static DateTimeInterval A { get; } = new DateTimeInterval(DateTime.Parse("2009-01-11T09:00:00+00:00"), DateTime.Parse("2009-01-11T10:00:00+00:00"));
        public static DateTimeInterval NoIntersectionWithA { get; } = new DateTimeInterval(DateTime.Parse("2009-01-11T10:00:00+00:00"), DateTime.Parse("2009-01-11T11:00:00+00:00"));
        public static DateTimeInterval IntersectsWithA { get; } = new DateTimeInterval(DateTime.Parse("2009-01-11T09:30:00+00:00"), DateTime.Parse("2009-01-11T11:00:00+00:00"));
        public static DateTimeInterval ContainsA { get; } = new DateTimeInterval(DateTime.Parse("2009-01-11T08:30:00+00:00"), DateTime.Parse("2009-01-11T11:00:00+00:00"));
        public static List<DateTimeInterval> ListA { get; } = new List<DateTimeInterval>()
        {
            A,
            ContainsA,
            NoIntersectionWithA,
            IntersectsWithA
        };


        public static IEnumerable<object[]> GetData(string testIndex)
        {
            var allData = new Dictionary<string, List<object[]>>()
            {
                { nameof(IntersectsWithA), new List<object[]>{new object[] { A, IntersectsWithA }}},
                { nameof(ContainsA), new List<object[]>{new object[] { A, ContainsA }}},
                { nameof(NoIntersectionWithA), new List<object[]>{new object[] { A, NoIntersectionWithA }}},
                { nameof(A), new List<object[]>{new object[] { A }}}
            };

            return allData[testIndex];
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(A))]
        public void Not(DateTimeInterval a)
        {
            var negation = (!a).ToList();
            Assert.Equal(2, negation.Count);
            Assert.Equal(DateTime.MinValue, negation[0].Start);
            Assert.Equal(A.Start, negation[0].Due);
            Assert.Equal(A.Due, negation[1].Start);
            Assert.Equal(DateTime.MaxValue, negation[1].Due);
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void CartesianProductNoIntersectionTest(DateTimeInterval a, DateTimeInterval b)
        {
            // no intersection
            Assert.Null(a & b);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void CartesianProductSubIntervalTest(DateTimeInterval a, DateTimeInterval b)
        {
            // a in b
            var r = a & b;
            Assert.NotNull(r);
            Assert.Equal(a.Start, r.Value.Start);
            Assert.Equal(a.Due, r.Value.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(IntersectsWithA))]
        public void CartesianProductWithIntersectionTest(DateTimeInterval a, DateTimeInterval b)
        {
            // a in b
            var r = a & b;
            Assert.NotNull(r);
            Assert.Equal(b.Start, r.Value.Start);
            Assert.Equal(a.Due, r.Value.Due);
        }
        [Fact]
        public void CartesianProductCommutativityTest()
        {
            Assert.Equal(A & ContainsA, ContainsA & A);
            Assert.Equal(A & NoIntersectionWithA, NoIntersectionWithA & A);
            Assert.Equal(A & IntersectsWithA, IntersectsWithA & A);
        }

        [Fact]
        public void CartesianProductListTest()
        {
            var r = A & ListA;
            Assert.NotEmpty(r);
            Assert.Equal(2, r.Count());
            Assert.Contains(r, x => x == A);
            Assert.Contains(r, x => x.Value.Start == IntersectsWithA.Start && x.Value.Due == A.Due);

            var s = ListA & A;
            Assert.NotEmpty(s);
            Assert.Equal(2, s.Count());
            Assert.Contains(s, x => x == A);
            Assert.Contains(s, x => x.Value.Start == IntersectsWithA.Start && x.Value.Due == A.Due);

        }


        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(IntersectsWithA))]
        public void UnionWithIntersectionTest(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a + b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Start, a.Start);
            Assert.Equal(i.Due, b.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void UnionWithNoIntersectionTest(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a + b;
            Assert.Equal(1, r.Count());
            var first = r.FirstOrDefault();
            Assert.Equal(first.Start, a.Start);
            Assert.Equal(first.Due, b.Due);
            r = a + new DateTimeInterval(b.Start.AddTicks(1), b.Due);
            Assert.Equal(2, r.Count());
            first = r.FirstOrDefault();
            Assert.Equal(first.Start, a.Start);
            Assert.Equal(first.Due, a.Due);

            var second = r.LastOrDefault();
            Assert.Equal(second.Start, b.Start.AddTicks(1));
            Assert.Equal(second.Due, b.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void UnionWithContainsTest(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a + b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Start, b.Start);
            Assert.Equal(i.Due, b.Due);
        }

        [Fact]
        public void UnionListTest()
        {
            var distinct = new List<DateTimeInterval>()
            {
                (DateTime.Parse("2009-11-01T08:00:00+01:00"), DateTime.Parse("2009-11-01T09:00:00+01:00")),
                (DateTime.Parse("2009-11-01T10:00:00+01:00"), DateTime.Parse("2009-11-01T12:00:00+01:00")),
                (DateTime.Parse("2009-11-01T14:00:00+01:00"), DateTime.Parse("2009-11-01T15:00:00+01:00")),
                (DateTime.Parse("2009-11-01T15:00:00+01:00"), DateTime.Parse("2009-11-01T17:00:00+01:00")),
            };
            var r = distinct[0] + distinct.Skip(1);
            for (int i = 0; i < distinct.Count; i++)
                Assert.Contains(r, x => x == distinct[i]);

            Assert.Null(A + (IEnumerable<DateTimeInterval>)null);
            Assert.Null((IEnumerable<DateTimeInterval>)null + A);
        }

        [Fact]
        public void UnionListCommutativityTest()
        {
            Assert.True((A + ListA.Skip(1)).All(x => (ListA.Skip(1) + A).Any(y => x == y)));
        }
        [Fact]
        public void UnionSameValueTest()
        {
            Assert.Single(A + A);
            Assert.Equal((A + A).FirstOrDefault(), A);
            Assert.Equal((ContainsA + A).FirstOrDefault(), ContainsA);
            Assert.Equal((A + ContainsA).FirstOrDefault(), ContainsA);
        }

        [Fact]
        public void UnionCommutativityTest()
        {
            Assert.True((A + ContainsA).All(x => (ContainsA + A).Any(y => x == y)));
            Assert.True((A + NoIntersectionWithA).All(x => (NoIntersectionWithA + A).Any(y => x == y)));
            Assert.True((A + IntersectsWithA).All(x => (IntersectsWithA + A).Any(y => x == y)));
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(IntersectsWithA))]
        public void ExceptIntersectWithA(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a - b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Value.Start, a.Start);
            Assert.Equal(i.Value.Due, b.Start);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void ExceptContainsA(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a - b;
            Assert.Empty(r);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void ExceptNoIntersectionWithA(DateTimeInterval a, DateTimeInterval b)
        {
            var r = a - b;
            Assert.Single(r);
            Assert.Equal(a, r.FirstOrDefault());
        }

        [Fact]
        public void ExcepListCommutativityTest()
        {
            var r = A - ListA;
            Assert.NotEmpty(r);
            Assert.True(r.All(x => (ListA - A).Any(y => x == y)));
        }

        [Fact]
        public void ExceptLeftListTest()
        {
            var r = ListA - A;
            Assert.NotEmpty(r);
            Assert.Equal(2, r.Count());
            Assert.Contains(r, x => x.Start == ContainsA.Start && x.Due == A.Start);
            Assert.Contains(r, x => x.Start == A.Due && x.Due == ContainsA.Due);
        }
        [Fact]
        public void ExceptListNullTest()
        {
            Assert.Single(A - (IEnumerable<DateTimeInterval>)null);
            Assert.Single((IEnumerable<DateTimeInterval>)null - A);
            Assert.Equal(A, (A - (IEnumerable<DateTimeInterval>)null).FirstOrDefault());
            Assert.Equal((A - (IEnumerable<DateTimeInterval>)null).FirstOrDefault(), ((IEnumerable<DateTimeInterval>)null - A).FirstOrDefault());
        }
        [Fact]
        public void EqualityTest()
        {
            Assert.True(A == new DateTimeInterval(A.Start, A.Due));
            Assert.True(A == A);
            Assert.True(A != new DateTimeInterval(A.Start, DateTime.MaxValue));
        }

        [Fact]
        public void GreaterThanTest()
        {
            Assert.False(A > new DateTimeInterval(A.Start, A.Due));
            Assert.False(A > new DateTimeInterval(A.Start, A.Due.AddTicks(1)));
            Assert.True(A > new DateTimeInterval(A.Start, A.Due.AddTicks(-1)));
            Assert.False(A > new DateTimeInterval(A.Start.AddTicks(1), A.Due));
            Assert.True(A > new DateTimeInterval(A.Start.AddTicks(-1), A.Due));
        }
        [Fact]
        public void LowerThanTest()
        {
            Assert.False(A < new DateTimeInterval(A.Start, A.Due));
            Assert.False(A < new DateTimeInterval(A.Start, A.Due.AddTicks(-1)));
            Assert.True(A < new DateTimeInterval(A.Start, A.Due.AddTicks(1)));
            Assert.False(A < new DateTimeInterval(A.Start.AddTicks(-1), A.Due));
            Assert.True(A < new DateTimeInterval(A.Start.AddTicks(1), A.Due));
        }


        [Fact]
        public void CompareTo()
        {
            Assert.Equal(0, A.CompareTo(new DateTimeInterval(A.Start, A.Due)));
            Assert.Equal(1, A.CompareTo(new DateTimeInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.Equal(-1, A.CompareTo(new DateTimeInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void EqualTest()
        {
            Assert.True(A.Equals(new DateTimeInterval(A.Start, A.Due)));
            Assert.False(A.Equals(new DateTimeInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.False(A.Equals(new DateTimeInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void EqualObjectTest()
        {
            Assert.False(A.Equals((object)null));

            Assert.True(A.Equals((object)new DateTimeInterval(A.Start, A.Due)));
            Assert.False(A.Equals((object)new DateTimeInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.False(A.Equals((object)new DateTimeInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void ImplicitConversionTest()
        {
            DateTimeInterval i = (A.Start, A.Due);
            DateTimeInterval j = new Tuple<DateTime, DateTime>(A.Start, A.Due);
            Assert.Equal(A, i);
            Assert.Equal(A, j);
        }

        [Fact]
        public void GetHashCodeTest()
        {
            Assert.Equal(A.GetHashCode(), (new DateTimeInterval(A.Start, A.Due)).GetHashCode());
            Assert.NotEqual(A.GetHashCode(), (new DateTimeInterval(A.Start, A.Due.AddTicks(1))).GetHashCode());
            Assert.NotEqual(A.GetHashCode(), (new DateTimeInterval(A.Start, A.Due.AddTicks(-1))).GetHashCode());
        }
        [Fact]
        public void IntersectsTest()
        {
            Assert.True(A | new DateTimeInterval(A.Start, A.Due));
            Assert.True(A | ContainsA);
            Assert.True(A | IntersectsWithA);
            Assert.True(A | NoIntersectionWithA);
            Assert.True(NoIntersectionWithA | A);
            Assert.True(IntersectsWithA | A);
            Assert.True(ContainsA | A);

            Assert.False(A | new DateTimeInterval(NoIntersectionWithA.Start.AddTicks(1), NoIntersectionWithA.Due));
            Assert.False(new DateTimeInterval(NoIntersectionWithA.Start.AddTicks(1), NoIntersectionWithA.Due) | A);

        }

        [Fact]
        public void InTest()
        {
            Assert.True(A ^ new DateTimeInterval(A.Start, A.Due));
            Assert.True(A ^ ContainsA);
            Assert.False(A ^ IntersectsWithA);
            Assert.False(A ^ NoIntersectionWithA);
        }

        [Fact]
        public void IsValiTest()
        {
            DateTimeInterval a = A;
            Assert.Throws<ArgumentException>("due", () => a = (A.Start, A.Start));
        }
    }
}