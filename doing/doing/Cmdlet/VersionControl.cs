//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    GetVersion.cs
// Content: GetVersion Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Management.Automation;

namespace Doing.Cmdlet
{
    [Cmdlet(VerbsData.Compare, "Version")]
    [OutputType(typeof(bool))]
    class CompareVersion : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Compare-Version";

        /// <summary>
        /// 输入Version
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public PSObject? InputObject { get; set; } = null;

        /// <summary>
        /// 输入条件表达式
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false)]
        public string Condition { get; set; } = "";

        protected override void ProcessRecord()
        {
            if (InputObject == null)
            {
                throw new DException.RuntimeException("The type of `InputObject` parameter is null!");
            }
            if (InputObject.BaseObject.GetType() != typeof(Version))
            {
                throw new DException.RuntimeException("The type of `InputObject` parameter isn't `Version`!");
            }
            if (string.IsNullOrEmpty(Condition))
            {
                throw new DException.RuntimeException("The `Condition` parameter is null or empty!");
            }

            Version current = (Version)InputObject.BaseObject;

            // 新于 || 等于
            if (Condition.StartsWith(">="))
            {
                Version value = Version.Parse(Condition[2..]);

                if (current.CompareTo(value) >= 0)
                {
                    WriteObject(true);
                    return;
                }
            }
            // 老于 || 等于
            else if (Condition.EndsWith("<="))
            {
                Version value = Version.Parse(Condition[2..]);

                if (current.CompareTo(value) <= 0)
                {
                    WriteObject(true);
                    return;
                }
            }
            // 新于
            else if (Condition.StartsWith(">"))
            {
                Version value = Version.Parse(Condition[1..]);

                if (current.CompareTo(value) > 0)
                {
                    WriteObject(true);
                    return;
                }
            }
            // 晚于
            else if (Condition.StartsWith("<"))
            {
                Version value = Version.Parse(Condition[1..]);

                if (current.CompareTo(value) < 0)
                {
                    WriteObject(true);
                    return;
                }
            }
            // 等于
            else if (Condition.StartsWith("="))
            {
                Version value = Version.Parse(Condition[1..]);

                if (current.CompareTo(value) == 0)
                {
                    WriteObject(true);
                    return;
                }
            }

            WriteObject(false);
        }
    }

    [Cmdlet(VerbsCommon.Get, "Version")]
    partial class GetVersion : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Get-Version";

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string ProgramName { get; set; } = "";

        /// <summary>
        /// 处理
        /// </summary>
        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(ProgramName))
            {
                throw new
                    DException.RuntimeException("The `ProgramName` parameter is null or empty!");
            }

            // 获取版本
            if (TryRegexVersion(ProgramName, out Version? output))
            {
                WriteObject(output);
            }
            // 错误
            else
            {
                WriteError(new ErrorRecord(
                    new DException.RuntimeException("Get version fail down!"),
                    "Get version fail down!",
                    ErrorCategory.InvalidArgument,
                    ProgramName));
            }
        }
    }

}
