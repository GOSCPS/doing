//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Variable.cs
// Content: Variable Source File
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
    /// <summary>
    /// 设置DoingVariable
    /// </summary>
    [Cmdlet(VerbsCommon.Set,"DVariale")]
    class SetDVariable : PSCmdlet
    {
        public const string CallName = "Set-DVariable";

        /// <summary>
        /// 变量值
        /// </summary>
        [Parameter(Position = 1,Mandatory = true, ValueFromPipeline = true)]
        public PSObject? InputObject { get; set; } = null;

        /// <summary>
        /// 变量名
        /// </summary>
        [Parameter(Position = 0,Mandatory = true, ValueFromPipeline = false)]
        public string? Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            // 检查参数
            if (string.IsNullOrEmpty(Name))
            {
                WriteError(Tool.ErrorHelper.NewError("The variable name is null or empty!", ErrorCategory.InvalidArgument, Name!));
                return;
            }
            
            if(InputObject == null)
            {
                WriteError(Tool.ErrorHelper.NewError("The variable value is null!", ErrorCategory.InvalidArgument, Name!));
                return;
            }

            // 获取运行空间
            if(!Engine.Runspace.TryGetRunspace(Host.InstanceId,out Engine.Runspace? runspace))
            {
                WriteError(Tool.ErrorHelper.NewError("Try to get doing runspace fail down!", ErrorCategory.InvalidArgument, Host.InstanceId));
            }

            // 设置
            runspace!.GlobalVariableTable.AddOrUpdate(Name!, InputObject!.BaseObject, (_, _) => { return InputObject.BaseObject; });
        }
    }

    /// <summary>
    /// 获取DoingVariable
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DVariale")]
    [OutputType(typeof(object))]
    class GetDVariable : PSCmdlet
    {
        public const string CallName = "Get-DVariable";

        /// <summary>
        /// 变量名
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string? Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            // 检查参数
            if (string.IsNullOrEmpty(Name))
            {
                WriteError(Tool.ErrorHelper.NewError("The variable name is null or empty!", ErrorCategory.InvalidArgument, Name!));
                return;
            }

            // 获取运行空间
            if (!Engine.Runspace.TryGetRunspace(Host.InstanceId, out Engine.Runspace? runspace))
            {
                WriteError(Tool.ErrorHelper.NewError("Try to get doing runspace fail down!", ErrorCategory.InvalidArgument, Host.InstanceId));
                return;
            }

            // 获取变量
            if(!runspace!.GlobalVariableTable.TryGetValue(Name,out object? value))
            {
                WriteError(Tool.ErrorHelper.NewError("The variable never seted!", ErrorCategory.InvalidArgument, Name));
                return;
            }

            // 输出
            WriteObject(value);
        }
    }


    /// <summary>
    /// 删除DoingVariable
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "DVariale")]
    class RemoveDVariable : PSCmdlet
    {
        public const string CallName = "Remove-DVariable";

        /// <summary>
        /// 变量名
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string? Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            // 检查参数
            if (string.IsNullOrEmpty(Name))
            {
                WriteError(Tool.ErrorHelper.NewError("The variable name is null or empty!", ErrorCategory.InvalidArgument, Name!));
                return;
            }

            // 获取运行空间
            if (!Engine.Runspace.TryGetRunspace(Host.InstanceId, out Engine.Runspace? runspace))
            {
                WriteError(Tool.ErrorHelper.NewError("Try to get doing runspace fail down!", ErrorCategory.InvalidArgument, Host.InstanceId));
                return;
            }

            // 删除变量
            runspace!.GlobalVariableTable.TryRemove(Name, out _);
        }
    }

    /// <summary>
    /// 检查DoingVariable
    /// </summary>
    [Cmdlet("Check", "DVariale")]
    [OutputType(typeof(bool))]
    class CheckDVariable : PSCmdlet
    {
        public const string CallName = "Check-DVariable";

        /// <summary>
        /// 变量名
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string? Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            // 检查参数
            if (string.IsNullOrEmpty(Name))
            {
                WriteError(Tool.ErrorHelper.NewError("The variable name is null or empty!", ErrorCategory.InvalidArgument, Name!));
                return;
            }

            // 获取运行空间
            if (!Engine.Runspace.TryGetRunspace(Host.InstanceId, out Engine.Runspace? runspace))
            {
                WriteError(Tool.ErrorHelper.NewError("Try to get doing runspace fail down!", ErrorCategory.InvalidArgument, Host.InstanceId));
                return;
            }

            // 检查变量是否定义
            WriteObject(runspace!.GlobalVariableTable.ContainsKey(Name));
        }
    }
}
