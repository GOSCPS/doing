//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Runspace.cs
// Content: Runspace Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.Concurrent;
using System.Management.Automation.Runspaces;

namespace Doing.Engine
{
    /// <summary>
    /// 运行空间
    /// </summary>
    public static class Runspace
    {
        /// <summary>
        /// 源文件
        /// </summary>
        public static ConcurrentDictionary<string, BuildFileInfo> SourceFile
        { get; } = new();

        /// <summary>
        /// 所有Target
        /// </summary>
        public static ConcurrentDictionary<string, Target> AllTargets
        { get; } = new();

        /// <summary>
        /// 目标Target
        /// </summary>
        public static ConcurrentDictionary<string, Target> AimTargets
        { get; } = new();

    }

    public class PowerShellRuner
    {
        /// <summary>
        /// 源文件基础行号
        /// </summary>
        public int BaseLine { get; init; }

        /// <summary>
        /// 源文件文件名
        /// </summary>
        public string BaseFile { get; init; }

        /// <summary>
        /// 输出流
        /// </summary>
        private readonly PSDataCollection<PSObject> pwshOutput = new();

        /// <summary>
        /// pwsh
        /// </summary>
        private PowerShell? shell = null;

        public PowerShellRuner(Target target)
        {
            BaseFile = target.DefineLine.Position.SourceFile.FullName;
            BaseLine = target.DefineLine.LineNumber;
        }

        /// <summary>
        /// 初始化Pwsh运行空间
        /// </summary>
        public static System.Management.Automation.Runspaces.Runspace CreateRunspace(
            InitialSessionState sessionState)
        {
            var spc = RunspaceFactory.CreateRunspace(sessionState);
            spc.Open();
            return spc;
        }

        /// <summary>
        /// 创建pwsh
        /// </summary>
        /// <returns></returns>
        public static void CreatePwsh(
            System.Management.Automation.Runspaces.Runspace runspace,
            PowerShellRuner runer)
        {
            PowerShell pwsh = PowerShell.Create(runspace);

            // 初始化
            // 将输出重定向
            pwsh.Streams.Error.DataAdded += (sender, args) =>
            {
                ErrorRecord err = ((PSDataCollection<ErrorRecord>)sender!)[args.Index];

                Tool.Printer.ErrLine(err.ToString().Replace("{", "{{").Replace("}", "}}"));
            };

            pwsh.Streams.Warning.DataAdded += (sender, args) =>
            {
                WarningRecord warning = ((PSDataCollection<WarningRecord>)sender!)[args.Index];

                Tool.Printer.WarnLine(warning.ToString().Replace("{", "{{").Replace("}", "}}"));
            };

            pwsh.Streams.Progress.DataAdded += (sender, args) =>
            {
                ProgressRecord progress = ((PSDataCollection<ProgressRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{progress.ToString().Replace("{", "{{")}");
            };

            pwsh.Streams.Information.DataAdded += (sender, args) =>
            {
                InformationRecord information = ((PSDataCollection<InformationRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{information.ToString().Replace("{", "{{")}");
            };

            pwsh.Streams.Verbose.DataAdded += (sender, args) =>
            {
                VerboseRecord verbose = ((PSDataCollection<VerboseRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{verbose.ToString().Replace("{", "{{")}");
            };

            runer.pwshOutput.DataAdded += (sender, args) =>
            {
                PSObject output = ((PSDataCollection<PSObject>)sender!)[args.Index];
                Tool.Printer.PutLine($"{output.ToString().Replace("{", "{{")}");
            };

            runer.shell = pwsh;

            return;
        }

        /// <summary>
        /// 执行一个Target
        /// </summary>
        /// <returns></returns>
        public static bool ExecuteTarget(Target target)
        {
            // 同一个targe只允许同时有一个在执行
            lock (target.RunLocker)
            {
                PowerShellRuner runer = new(target);
                InitialSessionState iss = InitialSessionState.CreateDefault();
                Cmdlet.StandardCmdlet.AddStandardCmdlet(iss);
                CreatePwsh(CreateRunspace(iss), runer);

                // 清除之前的输出和命令
                runer.shell!.Commands.Clear();
                runer.shell!.Streams.ClearStreams();
                runer.pwshOutput.Clear();

                // 调用命令
                runer.shell!.AddScript(target.Source);

                runer.shell!.Invoke(null, runer.pwshOutput);

                if (runer.shell!.HadErrors)
                    return false;

                else return true;
            }
        }
    }
}