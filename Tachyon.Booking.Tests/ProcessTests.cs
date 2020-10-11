using System;
using Tachyon.Booking.Scheduling;
using Tachyon.Booking.Time;
using Xunit;

namespace Tachyon.Booking.Tests
{
    public class ProcessTests
    {

        [Fact]
        public void BookingProcessTests()
        {
            var dateTimeProcess = new BookingProcess<DateTime>();
            var dateTimeOffsetProcess = new BookingProcess<DateTimeOffset>();
            var timeSpanProcess = new BookingProcess<TimeSpan>();


            Assert.Equal(typeof(DateTimeOffset), dateTimeOffsetProcess.BaseTemporalType);
            Assert.Equal(typeof(TimeSpan), timeSpanProcess.BaseTemporalType);
            Assert.Equal(typeof(DateTime), dateTimeProcess.BaseTemporalType);


            Assert.Equal(typeof(DateTimeOffsetInterval), dateTimeOffsetProcess.SupportedIntervalType);
            Assert.Equal(typeof(TimeInterval), timeSpanProcess.SupportedIntervalType);
            Assert.Equal(typeof(DateTimeInterval), dateTimeProcess.SupportedIntervalType);

        }
    }
}