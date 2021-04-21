//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Runspace.cs
// Content: Runspace Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using static Doing.Engine.Target;

namespace Doing.Engine
{
    /// <summary>
    /// 主Runspace
    /// </summary>
    public static class MainRunspace
    {
        /// <summary>
        /// Main
        /// </summary>
        private static readonly Runspace mainRunspace = new();

        /// <summary>
        /// 获取主运行空间
        /// </summary>
        /// <returns></returns>
        public static Runspace Get()
        {
            return mainRunspace;
        }
    }

    /// <summary>
    /// Target运行空间
    /// </summary>
    public class Runspace
    {
        /// <summary>
        /// 源文件
        /// key = FileInfo.FullName
        /// value = 对应文件名称的BuildFileInfo
        /// </summary>
        public ConcurrentDictionary<string, BuildFileInfo> SourceFile
        { get; } = new();

        /// <summary>
        /// 所有Target
        /// key = Target.Name,
        /// value = Target
        /// </summary>
        public ConcurrentDictionary<string, Target> AllTarget
        { get; } = new();

        /// <summary>
        /// 目标Target
        /// key = Target.name
        /// value = Target
        /// </summary>
        public ConcurrentDictionary<string, Target> AimTarget
        { get; } = new();

        /// <summary>
        /// 用户自定义函数
        /// key = Function.Name
        /// value = Function
        /// </summary>
        public ConcurrentDictionary<string, Function> AllFunction
        { get; } = new();

        /// <summary>
        /// 变量表
        /// </summary>
        public ConcurrentDictionary<string, object> GlobalVariableTable
        { get; } = new();

        /// <summary>
        /// 运行空间列表
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, Runspace>
            RunspaceList = new();

        /// <summary>
        /// 尝试获取运行空间
        /// </summary>
        /// 
        /// <param name="hostGuid">hostGuid</param>
        /// <param name="doingRunspace">doing的运行空间</param>
        /// 
        /// <returns>获取成功返回true，否则false</returns>
        public static bool TryGetRunspace(Guid hostGuid,out Runspace? doingRunspace)
        {
            if (RunspaceList.TryGetValue(hostGuid, out doingRunspace))
                return true;
            else return false;
        }

        /// <summary>
        /// 注册运行空间
        /// </summary>
        /// 
        /// <param name="hostGuid">hostGuid</param>
        /// 
        /// <returns>注册成功返回true，否则false</returns>
        public bool RegisteredRunspace(Guid hostGuid)
        {
            return RunspaceList.TryAdd(hostGuid, this);
        }

        /// <summary>
        /// 注销运行空间
        /// </summary>
        /// 
        /// <param name="hostGuid">hostGuid</param>
        public static void LayoutRunspace(Guid hostGuid)
        {
            RunspaceList.TryRemove(hostGuid, out _);
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="state"></param>
        public void AddFunctions(InitialSessionState state)
        {
            foreach (var func in AllFunction)
            {
                state.Commands.Add(new SessionStateFunctionEntry(func.Key, func.Value.Source));
            }
        }

        /// <summary>
        /// 初始化Pwsh运行空间
        /// </summary>
        public static System.Management.Automation.Runspaces.Runspace CreateRunspace(
            InitialSessionState sessionState,
            DHost host,
            string named = "DefaultRunspace")
        {
            var spc = RunspaceFactory.CreateRunspace(host, sessionState);
            spc.Open();
            spc.Name = named;

            return spc;
        }
    }

    public class TargetExecuter
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
        public readonly PSDataCollection<PSObject> pwshOutput = new();

        /// <summary>
        /// pwsh
        /// </summary>
        public PowerShell? shell = null;

        public TargetExecuter(Target target)
        {
            BaseFile = target.DefineLine.Position.SourceFile.FullName;
            BaseLine = target.DefineLine.LineNumber;
        }

        /// <summary>
        /// 创建pwsh
        /// </summary>
        /// <returns></returns>
        public static void CreatePwsh(
            System.Management.Automation.Runspaces.Runspace runspace,
            TargetExecuter runer)
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
    }
}