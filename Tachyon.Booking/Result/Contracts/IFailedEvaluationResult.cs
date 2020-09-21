using System;
using Tachyon.Booking.Handlers;

namespace Tachyon.Booking.Result.Contracts
{
    public interface IFailedEvaluationResult : IEvaluationResult
    {
        Exception Exception { get; set; }
        IMiddleware Source { get; set; }
    }
}