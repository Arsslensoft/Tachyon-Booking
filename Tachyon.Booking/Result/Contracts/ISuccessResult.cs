namespace Tachyon.Booking.Result.Contracts
{
    public interface ISuccessResult<TResult> : IEvaluationResult
    {
        TResult Result { get; set; }
    }
}