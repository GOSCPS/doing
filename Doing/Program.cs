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
    class Program
    {
        /// <summary>
        /// 是否在Debug模式下
        /// </summary>
        public static bool IsDebug { get; set; } = true;

        /// <summary>
        /// 全局锁
        /// </summary>
        public static object GlobalLocker = new object();

        /// <summary>
        /// 全局变量表
        /// </summary>
        public static Dictionary<string, string> GlobalKeyValuePairs
            = new Dictionary<string, string>();

        /// <summary>
        /// 全局Target
        /// </summary>
        public static List<string> GlobalTargets
            = new List<string>();

        /// <summary>
        /// 默认构建文件
        /// </summary>
        public static string RootFile = "make.doing";

        /// <summary>
        /// 线程计数器
        /// </summary>
        public static uint ThreadCount = 1;

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
                    return new Version(0, 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// 打印帮助
        /// </summary>
        public static void PrintHelp()
        {
            Tool.Printer.PutLine("Usage:doing [Option] [--Target]");
            Tool.Printer.PutLine("Option:");
            Tool.Printer.PutLine("\t-D[Key]=[Value]\t\tDefine key-value pair");
            Tool.Printer.PutLine("\t-F[FileName]\t\tDefine what file you want to build.");
            Tool.Printer.PutLine("\t-h\t\t\tGet help of Doing.");
            Tool.Printer.PutLine("\t-T[Number]\t\tSet the doing max build thread count.");
            Tool.Printer.PutLine("\t-Debug\t\t\tEnable the debug mode.");
        }

        static int Main(string[] args)
        {
            Tool.Printer.PutLine("*** Doing Build System ***");
            Tool.Printer.PutLine($"***   Version {DoingVersion}  ***");
            Tool.Printer.PutLine("***   Made by GOSCPS   ***");

            // 解析参数
            try
            {
                int ptr = 0;
                while (ptr != args.Length)
                {
                    // Debug模式
                    if (args[ptr] == "-Debug")
                    {
                        // 打印运行时错误堆栈
                        Engine.RuntimeException.PrintStack = true;
                        IsDebug = true;

                        Tool.Printer.PutLine("In Doing Debug Mode!");
                    }
                    // 定义变量
                    else if (args[ptr].StartsWith("-D"))
                    {
                        if (!args[ptr].Contains('='))
                        {
                            Tool.Printer.ErrLine($"Param `{args[ptr]}` usage error.");
                            return 1;
                        }

                        string def = args[ptr][2..];

                        if (!GlobalKeyValuePairs.ContainsKey(def[..def.IndexOf('=')]))
                            GlobalKeyValuePairs.Add(def[..def.IndexOf('=')], def[(def.IndexOf('=') + 1)..]);
                        else
                            Tool.Printer.WarnLine($"Global vars `{args[ptr]}` early defined.");
                    }
                    // 帮助
                    else if (args[ptr] == "--help" || args[ptr] == "-h")
                    {
                        PrintHelp();
                    }
                    // 获取Target
                    else if (args[ptr].StartsWith("--"))
                    {
                        if (!GlobalTargets.Contains(args[ptr][2..]))
                            GlobalTargets.Add(args[ptr][2..]);
                    }
                    // 文件
                    else if (args[ptr].StartsWith("-F"))
                    {
                        RootFile = args[ptr][2..].Trim();
                    }
                    // 线程数量
                    else if (args[ptr].StartsWith("-T"))
                    {
                        if (!uint.TryParse(args[ptr][2..], out ThreadCount))
                        {
                            ThreadCount = (uint)Math.Abs(Environment.ProcessorCount);
                            Tool.Printer.WarnLine($"Warn:Set thread count format error.Default use {ThreadCount}");
                        }
                    }
                    // 未知命令
                    else
                    {
                        Tool.Printer.ErrLine($"Unknown option:{args[ptr]}");
                        return 1;
                    }

                    ptr++;
                }
            }
            catch (Exception err)
            {
                Tool.Printer.ErrLine(err.ToString());
                return 1;
            }


            Stopwatch timer = new Stopwatch();

            timer.Start();
            Engine.Engine.Start();
            timer.Stop();

            Tool.Printer.PutLine($"Use " +
                $"{timer.Elapsed.Days} days {timer.Elapsed.Hours} hours {timer.Elapsed.Minutes} minutes " +
                $"{timer.Elapsed.Seconds} seconds {timer.Elapsed.Milliseconds} milliseconds");

            return 0;
        }
    }
}
