using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Persistence
{
    public interface IDataSource
    {
        IEnumerable<T> Get<T>();
        IEnumerable<T> Get<T>(Predicate<T> predicate);
    }
}
