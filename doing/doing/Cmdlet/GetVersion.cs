//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    GetVersion.cs
// Content: GetVersion Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace Doing.Cmdlet
{
    partial class GetVersion
    {
        /// <summary>
        /// 正则匹配表达式
        /// </summary>
        public const string VersionRegex = @"(?<Version>[0-9.]+)";

        private static readonly Regex versionRegex = new(VersionRegex);

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
            catch (Exception err)
            {
                WriteError(new ErrorRecord(
                err
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
        /// 将regex第一行匹配到正则表达式
        /// </summary>
        /// 
        /// <param name="programName"></param>
        /// <param name="output"></param>
        /// 
        /// <returns></returns>
        public bool TryRegexVersion(string programName, out Version? output)
        {
            output = null;

            // 启动程序
            // 参数 `--version`
            if (GetProcessOutput(programName, "--version", out StreamReader? reader, out _))
            {
                string line = reader!.ReadLine()!;

                if (versionRegex.IsMatch(line))
                {
                    // 匹配版本号正则表达式
                    Match match = versionRegex.Match(line);

                    // 解析
                    if (!Version.TryParse(match.Value, out output))
                    {
                        WriteError(Tool.ErrorHelper.NewError("The output version number of the program that cannot be confirmed!!",
                    ErrorCategory.InvalidOperation,
                    this));
                    }
                    else return true;

                }
                // 未匹配的正则表达式
                else
                {
                    WriteError(Tool.ErrorHelper.NewError("The output version number of the program that cannot be confirmed!!",
                    ErrorCategory.InvalidOperation,
                    this));
                }
            }
            // 启动程序失败
            else
            {
                WriteError(Tool.ErrorHelper.NewError("The program started fail down!",
                    ErrorCategory.InvalidOperation,
                    this));
                return false;
            }

            return true;
        }


    }
}
