using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Result.Enums;

namespace Tachyon.Booking.Result
{
    public abstract class EvaluationResult : IEvaluationResult
    {
        public EvaluationStatus Status { get; set; }
    }
}
