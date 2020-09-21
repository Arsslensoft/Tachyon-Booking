using System;
using System.Collections.Generic;
using System.Text;
using Tachyon.Booking.Context.Contracts;

namespace Tachyon.Booking.Policies
{
    /// <summary>
    /// Represents the base booking policy interface.
    /// </summary>
    public interface IPolicy
    {
        /// <summary>
        /// Indicates whether this policy can be ignored or not.
        /// </summary>
        bool CanBeIgnored(IBookingContext context);
        /// <summary>
        /// Indicates whether the policy is valid or not.
        /// </summary>
        bool IsValid(IBookingContext context);
    }
}
