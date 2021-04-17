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
        public static bool IsDebug {
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

        private static System.Collections.Concurrent.ConcurrentDictionary<string,string> GlobalVars_ = new();

        /// <summary>
        /// 变量表
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<string, string> GlobalVars
        {
            get
            {
                return GlobalVars_;
            }
            private set
            {
                GlobalVars_ = value;
            }
        }

        private static List<string> AimTargets_ = new();

        /// <summary>
        /// 构建目标
        /// </summary>
        public static List<string> AimTargets
        {
            get
            {
                return AimTargets_;
            }
            private set
            {
                AimTargets_ = value;
            }
        }


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
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        static void PrintInfo()
        {
            Tool.Printer.PutLine($"Doing version {DoingVersion}");
            Tool.Printer.PutLine($"Built-in PowerShell version {System.Management.Automation.PSVersionInfo.PSVersion}");
            
        }

        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="args">命令行参数</param>
        static int Main(string[] args)
        {
            {
                // 默认打印logo
                bool isPrintLogo = true;

                foreach (var arg in args)
                {

                    if (arg == "--help" || arg == "-h")
                    {
                        PrintHelp();
                        return 0;
                    }
                    else if (arg == "--version")
                    {
                        Tool.Printer.PutLine(DoingVersion.ToString());
                        return 0;
                    }
                    else if (arg == "--info")
                    {
                        PrintInfo();
                        return 0;
                    }
                    else if (arg == "--nologo")
                    {
                        isPrintLogo = false;
                    }
                    else if (arg == "--debug")
                    {
                        IsDebug = true;
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
                                return -1;
                            }
                            else ThreadCount = count;
                        }
                        else
                        {
                            Tool.Printer.ErrLine("The thread count isn't a right format!");
                            return -1;
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
                            return -1;
                        }
                    }
                    // 定义全局变量
                    else if (arg.StartsWith("-d"))
                    {
                        // 格式:-d[KEY=VALUE]
                        string keyValue = arg[2..];

                        if (!keyValue.Contains('='))
                        {
                            Tool.Printer.ErrLine($"The define usage error:{arg}");
                            return -1;
                        }

                        string key = keyValue[0..(keyValue.IndexOf('='))];
                        string value = keyValue[(keyValue.IndexOf('=') + 1)..];

                        if (GlobalVars.ContainsKey(key))
                        {
                            Tool.Printer.ErrLine($"The global variable is defined:{arg}");
                            return -1;
                        }
                        else GlobalVars.TryAdd(key, value);
                    }
                    // Target
                    else if (!arg.StartsWith('-'))
                    {
                        if (!AimTargets.Contains(arg))
                            AimTargets.Add(arg);
                    }
                    else
                    {
                        Tool.Printer.ErrLine($"Unknown param:{arg}");
                        return -1;
                    }
                }

                if (isPrintLogo)
                {
                    Tool.Printer.PutLine("*** Doing Build System ***");
                    Tool.Printer.PutLine($"***   Version {DoingVersion}  ***");
                    Tool.Printer.PutLine("***   Made by GOSCPS   ***");
                }
            }

            Stopwatch timer = new();
            timer.Start();

            try
            {
                Engine.Parser.Parsing();

                // 构建失败
                if (!Engine.Runner.StartRunner())
                {
                    timer.Stop();
                    Tool.Printer.ErrLine("Build error!");
                    Tool.Printer.PutLine($"Use {timer.Elapsed:G}");
                    return -1;
                }
            }
            catch (System.Exception err)
            {
                timer.Stop();
                Tool.Printer.ErrLine("Build error!");
                Tool.Printer.ErrLine(err.ToString().Replace("{","{{"));
                Tool.Printer.PutLine($"Use {timer.Elapsed:G}");
                return -1;
            }
            timer.Stop();
            Tool.Printer.OkLine("Build OK!");
            Tool.Printer.OkLine($"Use {timer.Elapsed:G}");


            return 0;
        }
    }
}
