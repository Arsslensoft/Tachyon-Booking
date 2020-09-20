using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Policies
{
    public interface IPolicy
    {
        bool CanBeIgnored { get; }
        bool IsValid { get; }
    }
}
