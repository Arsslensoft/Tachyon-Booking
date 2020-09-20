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
    }
}
