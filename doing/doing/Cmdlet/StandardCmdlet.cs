//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    StandardCmdlet.cs
// Content: StandardCmdlet Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Doing.Cmdlet
{
    /// <summary>
    /// 添加标准Cmdlet
    /// </summary>
    public static class StandardCmdlet
    {


        public static void AddStandardCmdlet(InitialSessionState state)
        {

            #region VersionControl
            state.Commands.Add(
                new SessionStateCmdletEntry(
                    CompareVersion.CallName,
                    typeof(CompareVersion),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    GetVersion.CallName,
                    typeof(GetVersion),
                    null));

            #endregion

            #region VariabelControl

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    SetDVariable.CallName,
                    typeof(SetDVariable),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    GetDVariable.CallName,
                    typeof(GetDVariable),
                    null));


            state.Commands.Add(
                new SessionStateCmdletEntry(
                    RemoveDVariable.CallName,
                    typeof(RemoveDVariable),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    CheckDVariable.CallName,
                    typeof(CheckDVariable),
                    null));

            #endregion

            #region IncCompile

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    AddOutput.CallName,
                    typeof(AddOutput),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    AddSource.CallName,
                    typeof(AddSource),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    CheckCompile.CallName,
                    typeof(CheckCompile),
                    null));

            #endregion

            #region System

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    GetAimTarget.CallName,
                    typeof(GetAimTarget),
                    null));

            state.Commands.Add(
                new SessionStateCmdletEntry(
                    CheckTargetFinish.CallName,
                    typeof(CheckTargetFinish),
                    null));

            #endregion
        }








    }
}
