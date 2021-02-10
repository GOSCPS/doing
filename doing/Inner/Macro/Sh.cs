/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Sh.cs
 * Content: Sh Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Sh", License = "GOSCPS", Version = 1)]
    public class Sh
    {
        public static (string, string) GetCommandLine(string param)
        {
            param = param.Replace("\"", "\\\"");
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return ("pwsh.exe", $" /c \"{param}\"");
            else
                return ("/bin/sh", $" -c \"{param}\"");
        }

        [Api.Macro("ShWithVar")]
        public bool ShWithVarMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            var cmd = GetCommandLine(param);

            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = cmd.Item1,
                    Arguments = cmd.Item2,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false
                }
            };

            //加入环境变量
            lock (Build.GlobalContext.GlobalContextLocker)
            {
                foreach(var v in Build.GlobalContext.GlobalEnvironmentVariables)
                {
                    proc.StartInfo.EnvironmentVariables.Add(v.Key, v.Value);
                }
            }
            if(interpreter != null)
                foreach (var v in interpreter.LocalVariables)
                {
                    //局部变量覆盖全局变量
                    if (proc.StartInfo.EnvironmentVariables.ContainsKey(v.Key))
                        proc.StartInfo.EnvironmentVariables.Remove(v.Key);

                    proc.StartInfo.EnvironmentVariables.Add(v.Key, v.Value);
                }

            //Start
            Printer.Common($"{cmd.Item1.Replace("{", "{{")} {cmd.Item2.Replace("{", "{{")}");

            proc.Start();
            proc.WaitForExit();

            if(proc.ExitCode != 0)
            {
                Printer.Error($"ShWithVarMacro Error:process return {proc.ExitCode}");
                return false;
            }

            return true;
        }

        [Api.Macro("Sh")]
        public bool ShMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            var cmd = GetCommandLine(param);

            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = cmd.Item1,
                    Arguments = cmd.Item2,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false
                }
            };

            //Start
            Printer.Common($"{cmd.Item1.Replace("{", "{{")} {cmd.Item2.Replace("{", "{{")}");

            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                Printer.Error($"ShWithVarMacro Error:process return {proc.ExitCode}");
                return false;
            }

            return true;
        }


    }
}
