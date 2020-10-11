using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Persistence;
using Tachyon.Booking.Policies;
using Tachyon.Booking.Policies.Attributes;
using Tachyon.Booking.Result.Contracts;
using Tachyon.Booking.Result.Enums;

namespace Tachyon.Booking.Handlers
{
    public abstract class BaseMiddleware : IMiddleware
    {
        public virtual IEnumerable<IPolicy> Policies { get; private set; } = new List<IPolicy>();
        public virtual IDataSource DataSource { get; private set; }
        public virtual IMiddleware Next { get; private set; }

        public virtual IMiddleware With<T>() where T : class, IMiddleware, new()
        {
            if (Next == null)
                Next = Activator.CreateInstance<T>();
            else throw new NotSupportedException($"This handler already has a middleware, {Next.GetType().Name}");
            // attach policies
            Policies = (from customAttribute in typeof(T).GetCustomAttributes<BookingPolicyAttribute>() where customAttribute.PolicyType != null select (IPolicy)Activator.CreateInstance(customAttribute.PolicyType)).ToList();
            return Next;
        }


        public virtual IMiddleware AttachDataSource<TDataSource>() where TDataSource : class, IDataSource, new()
        {
            this.DataSource = Activator.CreateInstance<TDataSource>();
            return this;
        }

        public virtual IEvaluationResult Evaluate<TResult>(IBookingContext context, IEvaluationResult previousEvaluation) where TResult : class
        {
            if (Policies.Any(policy => !policy.CanBeIgnored(context) && !policy.IsValid(context)))
                return Next.Evaluate<TResult>(context, previousEvaluation);

            var currentEvaluation = DoEvaluate<TResult>(context, previousEvaluation);
            if (Next != null)
                return ((currentEvaluation.Status & EvaluationStatus.Error) != EvaluationStatus.Error &&
                        (currentEvaluation.Status & EvaluationStatus.Override) != EvaluationStatus.Override)
                    ? Next.Evaluate<TResult>(context, currentEvaluation)
                    : currentEvaluation;
            else return currentEvaluation;
        }

        public virtual IMiddleware With<T, TDataSource>()
            where T : class, IMiddleware, new()
            where TDataSource : class, IDataSource, new()
         => With<T>().AttachDataSource<TDataSource>();


        public abstract IEvaluationResult DoEvaluate<TResult>(IBookingContext context, IEvaluationResult previousEvaluation) where TResult : class;
    }
}
