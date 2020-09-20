using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Time
{
    public interface IInterval<out T> where T : IEquatable<T>, IComparable<T>
    {
        T Start { get; }
        T Due { get; }

        bool IsValid { get; }
    }
}
