//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Target.cs
// Content: Target Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using static Doing.Engine.Target;

namespace Doing.Engine
{
    /// <summary>
    /// 构建行信息
    /// </summary>
    public class BuildLineInfo
    {
        /// <summary>
        /// 访问锁
        /// </summary>
        public object Locker { get; set; } = new();

        /// <summary>
        /// 行号
        /// </summary>
        public int LineNumber = 0;

        /// <summary>
        /// 源行
        /// </summary>
        public string Source = "";

        /// <summary>
        /// 源文件位置
        /// </summary>
        public BuildFileInfo Position { get; set; }

        public BuildLineInfo(BuildFileInfo pos)
        {
            Position = pos;
        }
    }

    /// <summary>
    /// 构建文件信息
    /// </summary>
    public class BuildFileInfo
    {
        /// <summary>
        /// 访问锁
        /// </summary>
        public object Locker { get; } = new();

        /// <summary>
        /// 源文件
        /// </summary>
        public FileInfo SourceFile { get; init; }

        /// <summary>
        /// 源文件行
        /// </summary>
        public BuildLineInfo[] AllLines { get; set; } = Array.Empty<BuildLineInfo>();

        /// <summary>
        /// Target列表
        /// </summary>
        public Target[] AllTargets { get; set; } = Array.Empty<Target>();

        /// <summary>
        /// 函数列表
        /// </summary>
        public Function[] AllFunction { get; set; } = Array.Empty<Function>();

        /// <summary>
        /// 主Main
        /// </summary>
        public Target? MainTarget = null;

        /// <summary>
        /// 运行空间
        /// </summary>
        public Runspace SourceRunspace { get; init; }

        /// <summary>
        /// Main是否已经构建过
        /// </summary>
        public bool IsMainBuilt = false;

        public BuildFileInfo(
            FileInfo source,
            Runspace runspace)
        {
            SourceFile = source;
            SourceRunspace = runspace;
        }

    }

    /// <summary>
    /// 包含依赖的可执行接口
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// 执行名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 执行依赖
        /// </summary>
        public string[] Deps { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// 
        /// <returns>执行成功返回true，否则false</returns>
        public bool Execute();
    }

    /// <summary>
    /// 目标
    /// </summary>
    public class Target : IExecutable
    {
        /// <summary>
        /// 访问锁
        /// </summary>
        public object RunLocker { get; } = new();

        /// <summary>
        /// Target名称
        /// </summary>
        public string Name { get; init; } = "#Unknown#";

        /// <summary>
        /// 定义行
        /// </summary>
        public BuildLineInfo DefineLine { get; init; }

        /// <summary>
        /// 依赖
        /// </summary>
        public string[] Deps { get; init; } = Array.Empty<string>();

        /// <summary>
        /// 源代码
        /// </summary>
        public string Source { get; init; } = "";

        /// <summary>
        /// 运行空间
        /// </summary>
        public Runspace TargetRunspace { get; init; }

        public Target(
            BuildLineInfo defined, Runspace runspace, string source, string name, string[] deps)
        {
            DefineLine = defined;
            Source = source;
            Name = name;
            Deps = deps;
            TargetRunspace = runspace;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public bool Execute()
        {
            lock (RunLocker)
            {
                // 初始化
                TargetExecuter runer = new(this);
                InitialSessionState iss = InitialSessionState.CreateDefault();
                DHost host = new(TargetRunspace);

                // 添加Cmdlet和Function
                Cmdlet.StandardCmdlet.AddStandardCmdlet(iss);
                TargetRunspace.AddFunctions(iss);

                // 创建运行空间
                var runsce = Runspace.CreateRunspace(iss, host, $"TargetRunspace-{Name}");

                // 创建pwsh
                TargetExecuter.CreatePwsh(runsce, runer);

                // 注册host
                TargetRunspace.RegisteredRunspace(host.InstanceId);

                // 清除之前的输出和命令
                runer.shell!.Commands.Clear();
                runer.shell!.Streams.ClearStreams();
                runer.pwshOutput.Clear();

                // 调用命令
                runer.shell!.AddScript(Source);

                runer.shell!.Invoke(null, runer.pwshOutput);

                // 关闭运行空间
                runer.shell.Runspace.Close();

                // 注销host
                Runspace.LayoutRunspace(host.InstanceId);

                // 调用错误
                if (runer.shell!.HadErrors)
                {
                    // 输出最后一个错误的详细信息 
                    ErrorRecord record = runer.shell.Streams.Error.Last();

                    // 包含位置信息
                    if (record.InvocationInfo.PositionMessage.Trim().Length != 0)
                    {
                        Tool.Printer.ErrLine("The last error line at {0} Lines {1}!",
                            DefineLine.Position.SourceFile.FullName,
                            record.InvocationInfo.ScriptLineNumber + DefineLine.LineNumber);
                    }

                    return false;
                }
                else return true;
            }
        }

        /// <summary>
        /// pwsh函数
        /// </summary>
        public class Function
        {
            /// <summary>
            /// Target名称
            /// </summary>
            public string Name { get; init; } = "#Unknown#";

            /// <summary>
            /// 定义行
            /// </summary>
            public BuildLineInfo DefineLine { get; init; }

            /// <summary>
            /// 源代码
            /// </summary>
            public string Source { get; init; } = "";

            /// <summary>
            /// 运行空间
            /// </summary>
            public Runspace FunctionRunspace { get; init; }

            public Function(
                BuildLineInfo defLine, Runspace runspace, string name, string body)
            {
                DefineLine = defLine;
                Name = name;
                Source = body;
                FunctionRunspace = runspace;
            }
        }

    }
}
