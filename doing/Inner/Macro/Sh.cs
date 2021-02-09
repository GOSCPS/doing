/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Sh.cs
 * Content: Sh Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Diagnostics;

namespace doing.Inner.Macro
{
    /// <summary>
    /// Sh命令
    /// </summary>
    [Api.DoingExpand("doing-InnerExpand.Macro.Sh", License = "GOSCPS", Version = 1)]
    public class Sh
    {
        //处理命令行参数
        private static ValueTuple<string, string> ProcessCommandLine(string param)
        {
            param.Replace("\"", "\\\"");

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return ("pwsh.exe", $"/c \"{param}\"");
            else
                return ("/bin/sh", $"-c \"{param}\"");
        }


        [Api.Macro("Sh")]
        public bool ShMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            var cmd = ProcessCommandLine(param);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd.Item1,
                    Arguments = cmd.Item2,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };
            //执行
            Printer.Common($"{process.StartInfo.FileName} " +
                $"{process.StartInfo.Arguments}");
            process.Start();
            process.WaitForExit();

            //不返回0视为错误
            if (process.ExitCode != 0)
            {
                Printer.Error($"Sh Error:command return {process.ExitCode}");
                return false;
            }
            return true;
        }

        [Api.Macro("ShWithVar")]
        public bool ShWithVarMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            var cmd = ProcessCommandLine(param);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd.Item1,
                    Arguments = cmd.Item2,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };

            //添加环境变量
            lock (Build.GlobalContext.GlobalContextLocker)
            {
                //全局变量
                foreach (var env in Build.GlobalContext.GlobalEnvironmentVariables)
                {
                    process.StartInfo.EnvironmentVariables.Add(env.Key, env.Value);
                }
            }

            //局部变量
            if (interpreter != null)
                foreach (var env in interpreter.LocalVariables)
                {
                    //局部覆盖全局变量
                    if (process.StartInfo.EnvironmentVariables.ContainsKey(env.Key))
                        process.StartInfo.EnvironmentVariables.Remove(env.Key);

                    process.StartInfo.EnvironmentVariables.Add(env.Key, env.Value);
                }

            //执行
            Printer.Common($"{process.StartInfo.FileName} " +
                $"{process.StartInfo.Arguments}");
            process.Start();
            process.WaitForExit();

            //不返回0视为错误
            if (process.ExitCode != 0)
            {
                Printer.Error($"Sh Error:command return {process.ExitCode}");
                return false;
            }
            return true;
        }

    }
}
