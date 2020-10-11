using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tachyon.Booking.Context;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Exceptions;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Result;
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

        public IEvaluationResult Evaluate<TResult, T>(string processName, T start, T due) where T : IEquatable<T>, IComparable<T>
            where TResult : class
        {
            var process = Processes.FirstOrDefault(x => x.Name == processName);
            if (process == null) throw new ProcessNotFoundException(processName);

            var context = new BookingContext<T>(start, due, process);
            return process.EntryPoint.Evaluate<TResult>(context, new NoneResult());
        }

        public IEvaluationResult Evaluate<TResult, TContext>(string processName, Func<BaseProcess, TContext> createContextCallback)
            where TContext : IBookingContext
            where TResult : class
        {
            var process = Processes.FirstOrDefault(x => x.Name == processName);
            if (process == null) throw new ProcessNotFoundException(processName);

            var context = createContextCallback(process);
            return process.EntryPoint.Evaluate<TResult>(context, new NoneResult());
        }
    }
}
