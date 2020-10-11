using System;
using System.Collections.Generic;
using System.Text;

namespace Tachyon.Booking.Exceptions
{
    public class ProcessNotFoundException : Exception
    {
        public ProcessNotFoundException(string processName)
        : base(string.Format(Constants.ProcessNotFoundMessage, processName))
        {

        }
    }
}
