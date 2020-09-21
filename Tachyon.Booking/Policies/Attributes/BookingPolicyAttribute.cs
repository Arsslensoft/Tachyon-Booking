using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Policies.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class BookingPolicyAttribute : Attribute
    {
        public Type PolicyType { get; set; }

        public BookingPolicyAttribute(Type policyType)
        {
            if (!typeof(IPolicy).IsAssignableFrom(policyType))
                throw new ArgumentException("The given policy does not implement from the IPolicy interface.", nameof(policyType));

            PolicyType = policyType;
        }
    }
}
