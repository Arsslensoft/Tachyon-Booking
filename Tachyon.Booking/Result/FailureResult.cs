using System;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Result.Enums;

namespace Tachyon.Booking.Result
{
    public class FailureResult<TResult> : EvaluationResult, IFailedEvaluationResult
    {
        public FailureResult()
        {
            Status = EvaluationStatus.Error;
        }
        public Exception Exception { get; set; }
        public IMiddleware Source { get; set; }
    }
}