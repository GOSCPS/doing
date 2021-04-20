//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    GetVersion.cs
// Content: GetVersion Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    [Cmdlet(VerbsData.Compare,"Version")]
    [OutputType(typeof(bool))]
    class CompareVersion : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Compare-Version";

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public PSObject? InputObject { get; set; } = null;

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false)]
        public string Condition { get; set; } = "";
        
        protected override void ProcessRecord()
        {
            if(InputObject == null)
            {
                throw new DException.RuntimeException("The type of `InputObject` parameter is null!");
            }
            if(InputObject.BaseObject.GetType() != typeof(Version))
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

                if(current.CompareTo(value) >= 0)
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

    [Cmdlet(VerbsCommon.Get,"Version")]
    class GetVersion : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Get-Version";

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string ProgramName { get; set; } = "";

        /// <summary>
        /// 获取进程输出
        /// </summary>
        /// 
        /// <returns>获取成功返回true，否则false</returns>
        public bool GetProcessOutput(
            string program,
            string args,
            out StreamReader? stdout,
            out StreamReader? stderr)
        {
            stdout = null;
            stderr = null;

            using Process process = new()
            {
                StartInfo = new()
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    FileName = program,
                    Arguments = args
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
            }
            // 程序启动失败
            catch (InvalidOperationException)
            {
                WriteError(new ErrorRecord(
                new DException.RuntimeException($"Try to start process `{program}` with args `{args}` fail down!")
                , $"Try to start process `{program}` with args `{args}` fail down!"
                , ErrorCategory.InvalidOperation
                , this));
                return false;
            }

            stdout = process.StandardOutput;
            stderr = process.StandardError;

            return true;
        }

        /// <summary>
        /// 获取Clang版本
        /// </summary>
        /// <param name="version">返回true则不为null</param>
        /// 
        /// <returns>获取成功返回true，否则false</returns>
        public bool TryGetClangVersion(out Version? version)
        {
            version = null;
            if(!GetProcessOutput("clang","--version",out StreamReader? stdout,out _))
            {
                return false;
            }
            else
            {
                string ver = stdout!.ReadLine()!;

                if (ver.Trim().StartsWith("clang version"))
                {
                    if (!Version.TryParse(ver["clang version".Length..].Trim(),out version))
                    {
                        WriteError(new ErrorRecord(
                        new DException.RuntimeException("The `clang --version` output is unexpected!")
                        , "The `clang --version` output is unexpected!"
                        , ErrorCategory.InvalidOperation
                        , this));
                        return false;
                    }
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new DException.RuntimeException("The `clang --version` output is unexpected!")
                        , "The `clang --version` output is unexpected!"
                        , ErrorCategory.InvalidOperation
                        , this));
                    return false;
                }

                return true;
            }
        }

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

            if(ProgramName.Trim().ToLower() == "clang")
            {
                if(TryGetClangVersion(out Version? ver))
                {
                    WriteObject(ver);
                }
                else
                {
                    WriteObject(new Version());
                }
            }
            else
            {
                WriteError(new ErrorRecord(
                    new DException.RuntimeException("Unknown program name!"),
                    "Unknown program name!",
                    ErrorCategory.InvalidArgument,
                    ProgramName));
            }
        }
    }

}
