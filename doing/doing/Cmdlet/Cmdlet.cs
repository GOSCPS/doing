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

            SessionStateCmdletEntry ssce = new(
                CheckLinux.CallName,
                typeof(CheckLinux),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckMac.CallName,
                typeof(CheckMac),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckWin.CallName,
                typeof(CheckWin),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckUnix.CallName,
                typeof(CheckUnix),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckClang.CallName,
                typeof(CheckClang),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                RemoveDoingVariable.CallName,
                typeof(RemoveDoingVariable),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
               SetDoingVariable.CallName,
               typeof(SetDoingVariable),
               null);
            iss.Commands.Add(ssce); 
            
            ssce = new(
                GetDoingVariable.CallName,
                typeof(GetDoingVariable),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckDoingVariable.CallName,
                typeof(CheckDoingVariable),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                CheckDoingBuiltTarget.CallName,
                typeof(CheckDoingBuiltTarget),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                GetDoingTotalTarget.CallName,
                typeof(GetDoingTotalTarget),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                GetDoingUserTarget.CallName,
                typeof(GetDoingUserTarget),
                null);
            iss.Commands.Add(ssce);

            ssce = new(
                AddDoingTarget.CallName,
                typeof(AddDoingTarget),
                null);
            iss.Commands.Add(ssce);

            return iss;
        }


    }
}
