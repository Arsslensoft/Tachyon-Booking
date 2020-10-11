using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Tachyon.Booking.Context;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Exceptions;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result;
using Tachyon.Booking.Scheduling;
using Tachyon.Booking.Time;
using Xunit;

namespace Tachyon.Booking.Tests
{
    public class SchedulerTests
    {
        private Scheduler Scheduler => new Scheduler();
        private BaseProcess Process => new BookingProcess<DateTimeOffset>()
        {
            Name = "Test Process"
        };

        (Scheduler scheduler, Mock<BaseMiddleware> first, Mock<BaseMiddleware> last, IBookingContext context, SuccessResult<DateTimeOffsetInterval> firstEval) SetupWithProcess()
        {
            var scheduler = Scheduler;
            var firstMiddleware = new Mock<BaseMiddleware>();
            var lastMiddleware = new Mock<BaseMiddleware>();
            var process = Process;
            var bookingContext = new BookingContext<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, process);

            var firstEvaluationResult = new SuccessResult<DateTimeOffsetInterval>()
            {
                Result = new DateTimeOffsetInterval(DateTimeOffset.MinValue, DateTimeOffset.MinValue.AddDays(10))
            };


            lastMiddleware.Setup(x => x.DoEvaluate<IEnumerable<DateTimeOffsetInterval>>(bookingContext, firstEvaluationResult)).Returns(new SuccessResult<DateTimeOffsetInterval>()
            {
                Result = new DateTimeOffsetInterval(DateTimeOffset.MinValue, DateTimeOffset.MinValue.AddDays(5))
            });

            firstMiddleware.Setup(x => x.DoEvaluate<IEnumerable<DateTimeOffsetInterval>>(bookingContext, null)).Returns(firstEvaluationResult);
            firstMiddleware.Setup(x => x.Next).Returns(lastMiddleware.Object);


            scheduler.Register(process, () => (firstMiddleware.Object, lastMiddleware.Object));

            return (scheduler, firstMiddleware, lastMiddleware, bookingContext, firstEvaluationResult);
        }
        [Fact]
        public void RegisterProcessTests()
        {
            var scheduler = Scheduler;
            var firstMiddleware = new Mock<IMiddleware>();
            var lastMiddleware = new Mock<IMiddleware>();
            firstMiddleware.Setup(x => x.Next).Returns(lastMiddleware.Object);

            scheduler.Register(Process, () => (firstMiddleware.Object, lastMiddleware.Object));
            var process = scheduler.Processes.FirstOrDefault();

            Assert.Single(scheduler.Processes);
            Assert.Equal(firstMiddleware.Object, process.EntryPoint);
            Assert.Equal(lastMiddleware.Object, process.Last);
        }

        [Fact]
        public void NoMatchProcessTests()
        {
            Assert.Throws<ProcessNotFoundException>(() =>
                Scheduler.Evaluate<IEnumerable<DateTimeOffsetInterval>, DateTimeOffset>("No", DateTimeOffset.MinValue,
                    DateTimeOffset.MaxValue));
        }
    }
}
