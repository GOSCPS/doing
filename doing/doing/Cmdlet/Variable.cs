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
    /// 获取全局变量
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DoingVariable")]
    class GetDoingVariable : PSCmdlet
    {
        public const string CallName = "Get-DoingVariable";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = false,Mandatory = true)]
        public string Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            if (Engine.Runner.GlobalDoingVariableTable.TryGetValue(Name, out object? value))
            {
                SessionState.PSVariable.Set(Name, value);
            }
            else throw new Exception.RuntimeException($"Couldn't found the variable `{Name}`!");
        }
    }

    /// <summary>
    /// 删除全局变量
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "DoingVariable")]
    class RemoveDoingVariable : PSCmdlet
    {
        public const string CallName = "Remove-DoingVariable";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true)]
        public string Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            Engine.Runner.GlobalDoingVariableTable.TryRemove(Name, out object? _);
        }
    }

    /// <summary>
    /// 判断全局变量是否设置
    /// </summary>
    [Cmdlet("Check", "DoingVariable")]
    class CheckDoingVariable : PSCmdlet
    {
        public const string CallName = "Check-DoingVariable";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true)]
        public string Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            if (Engine.Runner.GlobalDoingVariableTable.ContainsKey(Name))
                WriteObject(true);
            else
                WriteObject(false);
        }
    }

    /// <summary>
    /// 设置全局变量
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "DoingVariable")]
    class SetDoingVariable : PSCmdlet
    {
        public const string CallName = "Set-DoingVariable";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 1, ValueFromPipeline = true, Mandatory = true)]
        public object InputObject { get; set; } = new();

        protected override void ProcessRecord()
        {
            object obj = SessionState.PSVariable.GetValue(Name);
            Engine.Runner.GlobalDoingVariableTable.AddOrUpdate(
            Name, obj, (str, kobj) => { return InputObject; });
        }
    }

}
