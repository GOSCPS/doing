//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    System.cs
// Content: System Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    #region Get-AimTarget
    /// <summary>
    /// 获取运行空间目标Aim
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AimTarget")]
    [OutputType(typeof(string[]))]
    class GetAimTarget : PSCmdlet
    {
        public const string CallName = "Get-AimTarget";

        protected override void ProcessRecord()
        {
            // 获取运行空间
            if (!Engine.Runspace.TryGetRunspace(Host.InstanceId, out Engine.Runspace? runspace))
            {
                WriteError(Tool.ErrorHelper.NewError("Try to get doing runspace fail down!", ErrorCategory.InvalidArgument, Host.InstanceId));
                return;
            }

            // 检查变量是否定义
            WriteObject(runspace!.AimTargetQueue.ToArray());
        }
    }
    #endregion

    #region Check-TargetFinish
    /// <summary>
    /// 获取运行空间目标Aim
    /// </summary>
    [Cmdlet("Check", "TargetFinish")]
    [OutputType(typeof(bool))]
    class CheckTargetFinish : PSCmdlet
    {
        public const string CallName = "Check-TargetFinish";

        /// <summary>
        /// Target名
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false)]
        public string? Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            // 检查参数
            if (string.IsNullOrEmpty(Name))
            {
                WriteError(Tool.ErrorHelper.NewError("The target name is null or empty!", ErrorCategory.InvalidArgument, Name!));
                return;
            }

            Engine.DHost host = (Engine.DHost)Host;

            WriteObject(host.HostWorker.IsFinish(Name));
        }
    }
    #endregion






}
