using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Scheduling.Contracts;

namespace Tachyon.Booking.Scheduling
{
    public class Scheduler : IScheduler
    {
        public IEnumerable<BaseProcess> Processes { get; set; } = new HashSet<BaseProcess>();
        public void Register(BaseProcess process, Func<(IMiddleware first, IMiddleware last)> setupCallback)
        {
            var (first, last) = setupCallback();
            process.EntryPoint = first;
            process.Last = last;
            ((HashSet<BaseProcess>)Processes).Add(process);
        }

        public IEvaluationResult Evaluate<T>(string processName, T start, T due) where T : IEquatable<T>, IComparable<T>
        {
            throw new NotImplementedException();
        }
    }
}
