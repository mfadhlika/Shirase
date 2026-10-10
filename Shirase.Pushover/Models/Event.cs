using System;
using System.Collections.Generic;
using System.Text;

namespace Shirase.Pushover.Models
{
    public enum Event
    {
        Connected,
        Disconnected,
        Keepalive,
        NewMessage,
        ReloadRequest,
        PermanentError,
        SessionClosed,
    }
}
