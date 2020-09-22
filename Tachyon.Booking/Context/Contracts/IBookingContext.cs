using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Policies;
using Tachyon.Booking.Result;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Scheduling;

namespace Tachyon.Booking.Context.Contracts
{
    public interface IBookingContext
    {
        BaseProcess Process { get; }

        TResult GetResults<TResult>(IEvaluationResult result) where TResult : class;

    }
}
