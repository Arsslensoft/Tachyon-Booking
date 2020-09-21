using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Handlers
{
    public interface IMiddleware
    {
        IMiddleware Next { get; }
        IMiddleware With<T>() where T : IMiddleware;
    }
}
