using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Time;

namespace Tachyon.Booking.Persistence
{
    public interface IDataSource
    {
        IEnumerable<object> Get();
    }
}
