using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shirase.Pushover
{
    public class TwoFactorRequiredException: Exception
    {
        public TwoFactorRequiredException() { }

        public TwoFactorRequiredException(string message):base(message) { }

        public TwoFactorRequiredException(string message, Exception inner) : base(message) { }
    }

    public class DeviceNameAlreadyTakenException : Exception
    {
        public DeviceNameAlreadyTakenException() { }

        public DeviceNameAlreadyTakenException(string message) : base(message) { }

        public DeviceNameAlreadyTakenException(string message, Exception inner) : base(message) { }
    }
}
