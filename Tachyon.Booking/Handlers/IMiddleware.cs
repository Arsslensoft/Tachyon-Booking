using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Context;
using Tachyon.Booking.Context.Contracts;
using Tachyon.Booking.Persistence;
using Tachyon.Booking.Policies;
using Tachyon.Booking.Result;
using Tachyon.Booking.Result.Contracts;

namespace Tachyon.Booking.Handlers
{
    public interface IMiddleware
    {
        IEnumerable<IPolicy> Policies { get; }
        IDataSource DataSource { get; }
        IMiddleware Next { get; }
        IMiddleware With<T>() where T : class, IMiddleware, new();
        IMiddleware With<T, TDataSource>()
            where T : class, IMiddleware, new()
            where TDataSource : class, IDataSource, new();

        IMiddleware AttachDataSource<TDataSource>()
            where TDataSource : class, IDataSource, new();

        IEvaluationResult Evaluate<TResult>(IBookingContext context);
    }
}
