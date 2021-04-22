/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Program.cs
 * Content: Program Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Doing
{
    public class Program
    {
        private static bool IsDebug_ = false;
        /// <summary>
        /// 是否处于Debug模式
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                return IsDebug_;
            }
            private set
            {
                IsDebug_ = value;
            }
        }


        private static int ThreadCount_ = 1;
        /// <summary>
        /// 使用线程数量
        /// </summary>
        public static int ThreadCount
        {
            get
            {
                return ThreadCount_;
            }
            private set
            {
                ThreadCount_ = value;
            }
        }


        private static string BuildFile_ = "make.doing";
        /// <summary>
        /// 构建的文件
        /// </summary>
        public static string BuildFile
        {
            get
            {
                return BuildFile_;
            }
            private set
            {
                BuildFile_ = value;
            }
        }

        private static bool KeepGoOn_ = false;
        /// <summary>
        /// 忽略错误
        /// </summary>
        public static bool KeepGoOn
        {
            get
            {
                return KeepGoOn_;
            }
            private set
            {
                KeepGoOn_ = value;
            }
        }

        /// <summary>
        /// 变量表
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<string, string> GlobalVars
        {
            get;
        } = new();


        /// <summary>
        /// 构建目标
        /// </summary>
        public static List<string> AimTargets
        {
            get;
        } = new();


        /// <summary>
        /// 获取Doing版本号
        /// </summary>
        public static Version DoingVersion
        {
            get
            {
                try
                {
                    return
                         System.Reflection.Assembly.GetExecutingAssembly()!.GetName()!.Version!;
                }
                catch (NullReferenceException err)
                {
                    Tool.Printer.WarnLine(err.ToString());
                    return new(0, 0, 0, 0);
                }
            }
        }


        /// <summary>
        /// 打印帮助
        /// </summary>
        static void PrintHelp()
        {
            Tool.Printer.PutLine("Usage:doing [-(-)Option] [Target]");
            Tool.Printer.PutLine("Option:");
            Tool.Printer.PutLine("\t-d[Key]=[Value]\t\tDefine key-value pair.");
            Tool.Printer.PutLine("\t-f[FileName]\t\tDefine what file you want to build.");
            Tool.Printer.PutLine("\t-h|--help\t\tGet help of Doing then exit.");
            Tool.Printer.PutLine("\t-t[Number]\t\tSet the doing max build thread count.");
            Tool.Printer.PutLine("\t--debug\t\t\tEnable the debug mode.");
            Tool.Printer.PutLine("\t--version\t\tPrint the doing version.");
            Tool.Printer.PutLine("\t--info\t\t\tPrint more info about doing.");
            Tool.Printer.PutLine("\t--nologo\t\tNot output the logo.");
            Tool.Printer.PutLine("\t--goon\t\tContinue when error.");
        }


        /// <summary>
        /// 打印信息
        /// </summary>
        static void PrintInfo()
        {
            Tool.Printer.PutLine($"Doing version {DoingVersion}");
            Tool.Printer.PutLine($"Built-in PowerShell version {System.Management.Automation.PSVersionInfo.PSVersion}");
        }


        static void ProcessArgs(string[] args)
        {

            // 默认打印logo
            bool isPrintLogo = true;

            foreach (var arg in args)
            {
                // 一些小杂碎的事情
                if (arg == "--help" || arg == "-h")
                {
                    PrintHelp();
                    Environment.Exit(0);
                }
                else if (arg == "--version")
                {
                    Tool.Printer.PutLine(DoingVersion.ToString());
                    Environment.Exit(0);
                }
                else if (arg == "--info")
                {
                    PrintInfo();
                    Environment.Exit(0);
                }
                else if (arg == "--nologo")
                {
                    isPrintLogo = false;
                }
                else if (arg == "--debug")
                {
                    IsDebug = true;
                }
                else if (arg == "--goon")
                {
                    KeepGoOn_ = true;
                }
                // 指定线程数量
                else if (arg.StartsWith("-t"))
                {
                    string num = arg[2..];

                    if (int.TryParse(num, out int count))
                    {
                        // 确保线程数量不小于等于0
                        if (count <= 0)
                        {
                            Tool.Printer.ErrLine("The thread count isn't more then one!");
                            Environment.Exit(-1);
                        }
                        else ThreadCount = count;
                    }
                    // 错误的格式
                    else
                    {
                        Tool.Printer.ErrLine("The thread count isn't a right format!");
                        Environment.Exit(-1);
                    }
                }
                // 指定构建文件
                else if (arg.StartsWith("-f"))
                {
                    BuildFile = arg[2..];

                    // 不接受空文件名称
                    if (string.IsNullOrEmpty(BuildFile_))
                    {
                        Tool.Printer.ErrLine("The file name is empty!");
                        Environment.Exit(-1);
                    }
                }
                // 定义全局变量
                else if (arg.StartsWith("-d"))
                {
                    // 格式:-d[KEY=VALUE]
                    string keyValue = arg[2..];

                    // 没有=
                    if (!keyValue.Contains('='))
                    {
                        Tool.Printer.NoFormatErrLine($"The define usage error:{arg}");
                        Environment.Exit(-1);
                    }

                    string key = keyValue[0..(keyValue.IndexOf('='))];
                    string value = keyValue[(keyValue.IndexOf('=') + 1)..];

                    // 变量已经存在
                    if (GlobalVars.ContainsKey(key))
                    {
                        Tool.Printer.NoFormatErrLine($"The global variable is defined:{arg}");
                        Environment.Exit(-1);
                    }
                    else GlobalVars.TryAdd(key, value);
                }
                // 非-开头
                // Target
                else if (!arg.StartsWith('-'))
                {
                    if (!AimTargets.Contains(arg))
                        AimTargets.Add(arg);

                    // Target已定义
                    else Tool.Printer.WarnLine("The target is defined:{0}", arg);
                }
                // 位置指令
                else
                {
                    Tool.Printer.NoFormatErrLine($"Unknown param:{arg}");
                    Environment.Exit(-1);
                }
            }

            // 打印logo
            if (isPrintLogo)
            {
                Tool.Printer.PutLine("*** Doing Build System ***");
                Tool.Printer.PutLine($"***   Version {DoingVersion}  ***");
                Tool.Printer.PutLine("***   Made by GOSCPS   ***");
            }

            return;
        }

        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="args">命令行参数</param>
        static int Main(string[] args)
        {
            // 解析参数
            ProcessArgs(args);

            Stopwatch timer = new();
            timer.Start();

            // 执行
            try
            {
                // 解析
                Engine.Parsing.LoadFileToRunspace(
                    Engine.MainRunspace.Get(), BuildFile);

                if(AimTargets.Count == 0)
                {
                    Tool.Printer.WarnLine("No input target.Build `Default`.");
                    AimTargets.Add("Default");
                }

                // 分析依赖
                Engine.WorkerManager first = new(ThreadCount,KeepGoOn);
                Engine.WorkerManager last = new(ThreadCount, KeepGoOn);

                var queue = Engine.Parsing.MakeRunspaceDeps(Engine.MainRunspace.Get(),AimTargets);

                foreach(var f in queue.first)
                {
                    first.AddTask(f);
                }
                foreach (var l in queue.last)
                {
                    last.AddTask(l);
                }

                // 执行
                Engine.WorkerTeam team = new(KeepGoOn);

                team.WorkList.Enqueue(first);
                team.WorkList.Enqueue(last);

                if (!team.Execute())
                {
                    throw new DException.RuntimeException("Execute fail down!");
                }
            }
            catch (Exception err)
            {
                timer.Stop();
                Tool.Printer.ErrLine("Failed build!\nException:\n```");

                Tool.Printer.NoFormatErrLine(err.ToString());

                Tool.Printer.ErrLine("```");

                Tool.Printer.WarnLine("If you want to know more info.Use `--debug` parameter.");
                Tool.Printer.PutLine($"Use {timer.Elapsed:G}");
                return -1;
            }
            timer.Stop();
            Tool.Printer.OkLine("Successfully build!");
            Tool.Printer.OkLine($"Use {timer.Elapsed:G}");


            return 0;
        }
    }
}
