using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    /// <summary>
    /// Cmdlet总类
    /// </summary>
    public static class Cmdlet
    {

        public static InitialSessionState InitCmlet()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();

            SessionStateCmdletEntry ssce = new SessionStateCmdletEntry(
                "Check-Linux",
                typeof(CheckLinux),
                null);
            iss.Commands.Add(ssce);

            ssce = new SessionStateCmdletEntry("Check-Mac",
                typeof(CheckMac),
                null);
            iss.Commands.Add(ssce);

            ssce = new SessionStateCmdletEntry("Check-Win",
                typeof(CheckWin),
                null);
            iss.Commands.Add(ssce);

            ssce = new SessionStateCmdletEntry("Check-Unix",
                typeof(CheckUnix),
                null);
            iss.Commands.Add(ssce);


            return iss;
        }


    }
}
