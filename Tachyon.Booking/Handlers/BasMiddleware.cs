using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Handlers
{
    public abstract class BasMiddleware : IMiddleware
    {
        public IMiddleware Next { get; private set; }
        public IMiddleware With<T>() where T : IMiddleware
        {
            if (Next == null)
                Next = Activator.CreateInstance<T>();
            else throw new NotSupportedException($"This handler already has a middleware, {Next.GetType().Name}");

            return Next;
        }
    }
}
