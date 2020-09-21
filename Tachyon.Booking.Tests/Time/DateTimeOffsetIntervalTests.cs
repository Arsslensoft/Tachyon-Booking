using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tachyon.Booking.Time;
using Xunit;

namespace Tachyon.Booking.Tests.Time
{

    public class DateTimeOffsetIntervalTests
    {
        public static DateTimeOffsetInterval A { get; } = new DateTimeOffsetInterval(DateTimeOffset.Parse("2009-01-11T09:00:00+00:00"), DateTimeOffset.Parse("2009-01-11T10:00:00+00:00"));
        public static DateTimeOffsetInterval NoIntersectionWithA { get; } = new DateTimeOffsetInterval(DateTimeOffset.Parse("2009-01-11T10:00:00+00:00"), DateTimeOffset.Parse("2009-01-11T11:00:00+00:00"));
        public static DateTimeOffsetInterval IntersectsWithA { get; } = new DateTimeOffsetInterval(DateTimeOffset.Parse("2009-01-11T09:30:00+00:00"), DateTimeOffset.Parse("2009-01-11T11:00:00+00:00"));
        public static DateTimeOffsetInterval ContainsA { get; } = new DateTimeOffsetInterval(DateTimeOffset.Parse("2009-01-11T08:30:00+00:00"), DateTimeOffset.Parse("2009-01-11T11:00:00+00:00"));
        public static List<DateTimeOffsetInterval> ListA { get; } = new List<DateTimeOffsetInterval>()
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
        public void Not(DateTimeOffsetInterval a)
        {
            var negation = (!a).ToList();
            Assert.Equal(2, negation.Count);
            Assert.Equal(DateTimeOffset.MinValue, negation[0].Start);
            Assert.Equal(A.Start, negation[0].Due);
            Assert.Equal(A.Due, negation[1].Start);
            Assert.Equal(DateTimeOffset.MaxValue, negation[1].Due);
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void CartesianProductNoIntersectionTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            // no intersection
            Assert.Null(a & b);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void CartesianProductSubIntervalTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            // a in b
            var r = a & b;
            Assert.NotNull(r);
            Assert.Equal(a.Start, r.Value.Start);
            Assert.Equal(a.Due, r.Value.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(IntersectsWithA))]
        public void CartesianProductWithIntersectionTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
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
        }


        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(IntersectsWithA))]
        public void UnionWithIntersectionTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            var r = a + b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Start, a.Start);
            Assert.Equal(i.Due, b.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void UnionWithNoIntersectionTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            var r = a + b;
            Assert.Equal(2, r.Count());
            var first = r.FirstOrDefault();
            var second = r.LastOrDefault();
            Assert.Equal(first.Start, a.Start);
            Assert.Equal(first.Due, a.Due);
            Assert.Equal(second.Start, b.Start);
            Assert.Equal(second.Due, b.Due);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void UnionWithContainsTest(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            var r = a + b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Start, b.Start);
            Assert.Equal(i.Due, b.Due);
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
        public void ExceptIntersectWithA(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            var r = a - b;
            Assert.Single(r);
            var i = r.FirstOrDefault();
            Assert.Equal(i.Value.Start, a.Start);
            Assert.Equal(i.Value.Due, b.Start);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(ContainsA))]
        public void ExceptContainsA(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
        {
            var r = a - b;
            Assert.Empty(r);
        }
        [Theory]
        [MemberData(nameof(GetData), parameters: nameof(NoIntersectionWithA))]
        public void ExceptNoIntersectionWithA(DateTimeOffsetInterval a, DateTimeOffsetInterval b)
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
            Assert.Null(A - (IEnumerable<DateTimeOffsetInterval>)null);
            Assert.Null((IEnumerable<DateTimeOffsetInterval>)null - A);
        }
        [Fact]
        public void EqualityTest()
        {
            Assert.True(A == new DateTimeOffsetInterval(A.Start, A.Due));
            Assert.True(A == A);
            Assert.True(A != new DateTimeOffsetInterval(A.Start, DateTimeOffset.MaxValue));
        }

        [Fact]
        public void GreaterThanTest()
        {
            Assert.False(A > new DateTimeOffsetInterval(A.Start, A.Due));
            Assert.False(A > new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1)));
            Assert.True(A > new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1)));
            Assert.False(A > new DateTimeOffsetInterval(A.Start.AddTicks(1), A.Due));
            Assert.True(A > new DateTimeOffsetInterval(A.Start.AddTicks(-1), A.Due));
        }
        [Fact]
        public void LowerThanTest()
        {
            Assert.False(A < new DateTimeOffsetInterval(A.Start, A.Due));
            Assert.False(A < new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1)));
            Assert.True(A < new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1)));
            Assert.False(A < new DateTimeOffsetInterval(A.Start.AddTicks(-1), A.Due));
            Assert.True(A < new DateTimeOffsetInterval(A.Start.AddTicks(1), A.Due));
        }


        [Fact]
        public void CompareTo()
        {
            Assert.Equal(0, A.CompareTo(new DateTimeOffsetInterval(A.Start, A.Due)));
            Assert.Equal(1, A.CompareTo(new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.Equal(-1, A.CompareTo(new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void EqualTest()
        {
            Assert.True(A.Equals(new DateTimeOffsetInterval(A.Start, A.Due)));
            Assert.False(A.Equals(new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.False(A.Equals(new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void EqualObjectTest()
        {
            Assert.True(A.Equals((object)new DateTimeOffsetInterval(A.Start, A.Due)));
            Assert.False(A.Equals((object)new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1))));
            Assert.False(A.Equals((object)new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1))));
        }

        [Fact]
        public void ImplicitConversionTest()
        {
            DateTimeOffsetInterval i = (A.Start, A.Due);
            DateTimeOffsetInterval j = (A.Start, A.Due);
            Assert.Equal(A, i);
            Assert.Equal(A, j);
        }

        [Fact]
        public void GetHashCodeTest()
        {
            Assert.Equal(A.GetHashCode(), (new DateTimeOffsetInterval(A.Start, A.Due)).GetHashCode());
            Assert.NotEqual(A.GetHashCode(), (new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(1))).GetHashCode());
            Assert.NotEqual(A.GetHashCode(), (new DateTimeOffsetInterval(A.Start, A.Due.AddTicks(-1))).GetHashCode());
        }


    }
}
