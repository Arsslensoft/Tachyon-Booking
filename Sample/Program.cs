using System;
using System.Collections.Generic;
using System.Linq;
using Tachyon.Booking.Context;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Handlers;
using Tachyon.Booking.Persistence;
using Tachyon.Booking.Policies;
using Tachyon.Booking.Policies.Attributes;
using Tachyon.Booking.Result;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Scheduling;
using Tachyon.Booking.Time;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new Scheduler();
            scheduler.Register(new BookingProcess<DateTimeOffset>() { Name = "HELLO" }, () =>
               {
                   var first = new MiddlewareA();

                   var last = first.AttachDataSource<DataLoaderA>()
                       .With<MiddlewareB, DataLoaderB>();
                   return (first, last);
               });
            var res = scheduler.Evaluate<IEnumerable<DateTimeOffsetInterval>, DateTimeOffset>("HELLO", DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
        }
    }

    class DataLoaderA : IDataSource
    {
        public IEnumerable<object> Get()
        {
            yield return new DateTimeOffsetInterval(DateTimeOffset.Now, DateTimeOffset.MaxValue);
        }
    }
    class DataLoaderB : IDataSource
    {
        public IEnumerable<object> Get()
        {
            yield return new DateTimeOffsetInterval(DateTimeOffset.MinValue, DateTimeOffset.Now);
        }
    }
    class BasicPolicy : IPolicy
    {
        public bool CanBeIgnored(IBookingContext context)
        {
            return false;
        }

        public bool IsValid(IBookingContext context)
        {
            return true;
        }
    }
    [BookingPolicy(typeof(BasicPolicy))]
    class MiddlewareA : BaseMiddleware
    {
        public override IEvaluationResult DoEvaluate<TResult>(IBookingContext context, IEvaluationResult previousEvaluation) where TResult : class
        {
            Console.WriteLine(GetType().Name);
            var data = DataSource.Get().Cast<DateTimeOffsetInterval>().ToList();
            var previousResult = context.GetResults<TResult>(previousEvaluation);
            Console.WriteLine("Previous " + previousResult);
            return new SuccessResult<IEnumerable<DateTimeOffsetInterval>>()
            {
                Result = data
            };
        }
    }
    class MiddlewareB : BaseMiddleware
    {
        public override IEvaluationResult DoEvaluate<TResult>(IBookingContext context, IEvaluationResult previousEvaluation) where TResult : class
        {
            Console.WriteLine(GetType().Name);
            var data = DataSource.Get().Cast<DateTimeOffsetInterval>().ToList();
            var previousResult = context.GetResults<TResult>(previousEvaluation);
            Console.WriteLine("Previous " + previousResult);
            return new SuccessResult<IEnumerable<DateTimeOffsetInterval>>()
            {
                Result = ((IEnumerable<DateTimeOffsetInterval>)previousResult).FirstOrDefault() + data.FirstOrDefault()
            };
        }
    }
}
