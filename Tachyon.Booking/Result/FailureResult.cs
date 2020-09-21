using System;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result.Contracts;

namespace Tachyon.Booking.Result
{
    public class FailureResult<TResult> : EvaluationResult, IFailedEvaluationResult
    {
        public Exception Exception { get; set; }
        public IMiddleware Source { get; set; }
    }
}