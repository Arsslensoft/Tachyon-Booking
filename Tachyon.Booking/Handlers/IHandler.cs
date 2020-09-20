using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Handlers
{
    public interface IHandler
    {
        IHandler Next { get; }
        IHandler With<T>() where T : IHandler;
    }
}
