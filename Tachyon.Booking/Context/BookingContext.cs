using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Policies;
using Tachyon.Booking.Result;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Scheduling;

namespace Tachyon.Booking.Context
{
    public class BookingContext<T> : IBookingContext
        where T : IEquatable<T>, IComparable<T>
    {
        public T Start { get; }
        public T Due { get; }
        public BaseProcess Process { get; }
        public TResult GetResults<TResult>(IEvaluationResult result) where TResult : class
        {
            if (result is SuccessResult<TResult> successResult)
                return successResult.Result;
            else return null;
        }

        public BookingContext(T start, T due, BaseProcess process)
        {
            Start = start;
            Due = due;
            Process = process;
        }
    }
}
