using Tachyon.Booking.Result.Contracts;

namespace Tachyon.Booking.Result
{
    public class SuccessResult<TResult> : EvaluationResult, ISuccessResult<TResult>
    {
        public TResult Result { get; set; }
    }
}