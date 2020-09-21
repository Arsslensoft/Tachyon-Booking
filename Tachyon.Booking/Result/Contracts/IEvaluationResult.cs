using Tachyon.Booking.Result.Enums;

namespace Tachyon.Booking.Result.Contracts
{
    public interface IEvaluationResult
    {
        EvaluationStatus Status { get; set; }
    }
}