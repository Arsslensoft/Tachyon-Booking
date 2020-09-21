using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Time;

namespace Tachyon.Booking.Scheduling.Contracts
{
    public interface IScheduler
    {
        IEnumerable<BaseProcess> Processes { get; set; }
        void Register(BaseProcess process, Func<(IMiddleware first, IMiddleware last)> setupCallback);
        IEvaluationResult Evaluate<T>(string processName, T start, T due) where T : IEquatable<T>, IComparable<T>;
    }
}
