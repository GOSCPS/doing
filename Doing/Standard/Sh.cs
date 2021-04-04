/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Sh.cs
 * Content: Sh Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System.Management.Automation;


namespace Doing.Standard
{
    /// <summary>
    /// 执行shell命令
    /// </summary>
    class Sh : Engine.Utility.Function
    {
        /// <summary>
        /// 本地object表pwsh变量名称
        /// </summary>
        public const string LocalPwshVarName = "__DOING__PWSH__";

        /// <summary>
        /// 创建新PowerShell
        /// </summary>
        public static (PowerShell, PSDataCollection<PSObject>) CreatePowerShell()
        {
            // 新shell
            PowerShell shell = PowerShell.Create();

            // 初始化
            // 将输出同时定向到output
            shell.Streams.Error.DataAdded += (sender, args) =>
            {
                ErrorRecord err = ((PSDataCollection<ErrorRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{err}");
            };

            shell.Streams.Warning.DataAdded += (sender, args) =>
            {
                WarningRecord warning = ((PSDataCollection<WarningRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{warning}");
            };

            shell.Streams.Progress.DataAdded += (sender, args) =>
            {
                ProgressRecord progress = ((PSDataCollection<ProgressRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{progress}");
            };

            shell.Streams.Information.DataAdded += (sender, args) =>
            {
                InformationRecord information = ((PSDataCollection<InformationRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{information}");
            };

            shell.Streams.Verbose.DataAdded += (sender, args) =>
            {
                VerboseRecord verbose = ((PSDataCollection<VerboseRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{verbose}");
            };

            var result = new PSDataCollection<PSObject>();
            result.DataAdded += (sender, args) =>
            {
                PSObject output = ((PSDataCollection<PSObject>)sender!)[args.Index];
                Tool.Printer.PutLine($"{output}");
            };

            return (shell, result);
        }

        /// <summary>
        /// 执行pwsh命令
        /// </summary>
        /// 
        /// <param name="cmd"></param>
        /// 
        /// <returns>执行成功返回true，否则false</returns>
        public static bool ExecutePwsh(string script, (PowerShell, PSDataCollection<PSObject>) pwsh)
        {
            // 清除之前的输出和命令
            pwsh.Item1.Commands.Clear();
            pwsh.Item1.Streams.ClearStreams();
            pwsh.Item2.Clear();

            pwsh.Item1.AddScript(script);

            pwsh.Item1.Invoke(null, pwsh.Item2);

            if (pwsh.Item1.HadErrors)
                return false;
            else return true;
        }

        // shell
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            string commandS = new Format().Execute(callerContext, args).ValueString;

            // 获取powershell
            (PowerShell, PSDataCollection<PSObject>) shell;

            if (callerContext.LocalObjectTable.TryGetValue(LocalPwshVarName, out object? value))
            {
                // shell复用
                shell = ((PowerShell, PSDataCollection<PSObject>))(value);
            }
            else
            {
                shell = CreatePowerShell();

                callerContext.LocalObjectTable.TryAdd(LocalPwshVarName, shell);
            }

            // 执行
            if (Program.IsDebug)
                Tool.Printer.PutLine("pwsh & " + commandS.Replace("{", "{{").Replace("}", "}}"));

            if (ExecutePwsh(commandS, shell))
            {
                return new Variable();
            }
            else throw new RuntimeException("Shell Command execute error!");
        }
    }

    class Export : Engine.Utility.Function
    {

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            // 收集字符串参数
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");


            // 获取powershell
            (PowerShell, PSDataCollection<PSObject>) shell;

            if (callerContext.LocalObjectTable.TryGetValue(Sh.LocalPwshVarName, out object? value))
            {
                // shell复用
                shell = ((PowerShell, PSDataCollection<PSObject>))(value);
            }
            else
            {
                shell = Sh.CreatePowerShell();

                callerContext.LocalObjectTable.TryAdd(Sh.LocalPwshVarName, shell);
            }

            // 执行
            if (Program.IsDebug)
                Tool.Printer.PutLine("pwsh & " + $"$Env:Path=$Env:Path+\"{args[0].ValueString}\""
                    .Replace("{", "{{").Replace("}", "}}"));

            if (Sh.ExecutePwsh($"$Env:Path=$Env:Path+\"{args[0].ValueString}\"", shell))
            {
                return new Variable();
            }
            else throw new RuntimeException("Shell Command execute error!");
        }
    }

    class ExecuteScript : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            // 收集字符串参数
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");


            // 获取powershell
            (PowerShell, PSDataCollection<PSObject>) shell;

            if (callerContext.LocalObjectTable.TryGetValue(Sh.LocalPwshVarName, out object? value))
            {
                // shell复用
                shell = ((PowerShell, PSDataCollection<PSObject>))(value);
            }
            else
            {
                shell = Sh.CreatePowerShell();

                callerContext.LocalObjectTable.TryAdd(Sh.LocalPwshVarName, shell);
            }

            // 执行
            if (Program.IsDebug)
                Tool.Printer.PutLine("pwsh " + $"{args[0].ValueString}".Replace("{", "{{").Replace("}", "}}"));

            if (Sh.ExecutePwsh(args[0].ValueString, shell))
                return new Variable();
            else throw new RuntimeException("Shell Command execute error!");
        }
    }
}
