using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Result.Enums;

namespace Tachyon.Booking.Result
{
    public class NoneResult : EvaluationResult
    {

    }
    public class SuccessResult<TResult> : EvaluationResult, ISuccessResult<TResult>
    {
        public SuccessResult()
        {
            Status = EvaluationStatus.Success;
        }
        public TResult Result { get; set; }
    }
}