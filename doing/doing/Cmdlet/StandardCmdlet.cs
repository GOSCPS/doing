//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    StandardCmdlet.cs
// Content: StandardCmdlet Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    /// <summary>
    /// 添加标准Cmdlet
    /// </summary>
    public static class StandardCmdlet
    {


        public static void AddStandardCmdlet(InitialSessionState state)
        {

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
        }








    }
}
