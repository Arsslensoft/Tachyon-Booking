using System;

namespace Tachyon.Booking.Result.Enums
{
    [Flags]
    public enum EvaluationStatus
    {
        Success = 0x2,
        Error = 0x4,
        Override = 0x8
    }
}