using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Tachyon.Booking.Persistence
{
    public interface IPersistence : IDisposable
    {
        void Persist<T>(T meeting);
    }
}
