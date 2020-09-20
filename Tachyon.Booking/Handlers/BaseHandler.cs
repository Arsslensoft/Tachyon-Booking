using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Handlers
{
    public abstract class BaseHandler : IHandler
    {
        public IHandler Next { get; private set; }
        public IHandler With<T>() where T : IHandler
        {
            if (Next == null)
                Next = Activator.CreateInstance<T>();
            else throw new NotSupportedException($"This handler already has a middleware, {Next.GetType().Name}");

            return Next;
        }
    }
}
