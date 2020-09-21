using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Time;

namespace Tachyon.Booking.Scheduling
{
    public class BookingProcess<T> : BaseProcess
        where T : IEquatable<T>, IComparable<T>
    {
        public BookingProcess()
        {
            if (typeof(T) == typeof(TimeSpan))
                SupportedIntervalType = typeof(TimeInterval);
            else if (typeof(T) == typeof(DateTime))
                SupportedIntervalType = typeof(DateTimeInterval);
            else if (typeof(T) == typeof(DateTimeOffset))
                SupportedIntervalType = typeof(DateTimeOffsetInterval);

            BaseTemporalType = typeof(T);
        }
        public Type SupportedIntervalType { get; set; }
        public Type BaseTemporalType { get; set; }
    }
    public abstract class BaseProcess
    {
        public string Name { get; set; }
        public IMiddleware EntryPoint { get; set; }
        public IMiddleware Last { get; set; }

        //TODO: Implement base Tostring /get hash code / equals
    }
}
