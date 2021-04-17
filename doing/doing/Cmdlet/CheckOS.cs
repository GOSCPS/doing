using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace Doing.Cmdlet
{
    [Cmdlet("Check","Unix")]
    public class CheckUnix : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Unix";

        protected override void ProcessRecord()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                WriteObject(true);
            }
            else WriteObject(false);
        }
    }

    [Cmdlet("Check", "Win")]
    public class CheckWin : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Win";

        protected override void ProcessRecord()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WriteObject(true);
            }
            else WriteObject(false);
        }
    }

    [Cmdlet("Check", "Mac")]
    public class CheckMac : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Mac";

        protected override void ProcessRecord()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                WriteObject(true);
            }
            else WriteObject(false);
        }
    }

    [Cmdlet("Check", "Linux")]
    public class CheckLinux : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Linux";

        protected override void ProcessRecord()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                WriteObject(true);
            }
            else WriteObject(false);
        }
    }

}
