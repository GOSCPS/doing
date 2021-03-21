/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Program.cs
 * Content: Program Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Doing
{
    class Program
    {

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
                catch(NullReferenceException err)
                {
                    Tool.Printer.Warn(err.ToString());
                    return new Version(0,0,0,0);
                }
            } 
        }

        /// <summary>
        /// 打印帮助
        /// </summary>
        public static void PrintHelp()
        {
            Tool.Printer.Put("Usage:doing [Option] [--Target]");
            Tool.Printer.Put("Option:");
            Tool.Printer.Put("\t-D[Key]=[Value]\t\tDefine key-value pair");
            Tool.Printer.Put("\t-F[FileName]\t\tDefine what file you want to build.");
            Tool.Printer.Put("\t-h\t\t\tGet help of Doing.");
        }

        static int Main(string[] args)
        {
            Tool.Printer.Put("*** Doing Build System ***");
            Tool.Printer.Put($"***   Version {DoingVersion}  ***");
            Tool.Printer.Put("***   Made by GOSCPS   ***");

            // 解析参数
            try
            {
                int ptr = 0;
                while (ptr != args.Length)
                {

                    // 定义变量
                    if (args[ptr].StartsWith("-D"))
                    {
                        if (!args[ptr].Contains('='))
                        {
                            Tool.Printer.Err($"Param `{args[ptr]}` usage error.");
                            return 1;
                        }

                        string def = args[ptr][2..];

                        if (!GlobalKeyValuePairs.ContainsKey(def[..def.IndexOf('=')]))
                            GlobalKeyValuePairs.Add(def[..def.IndexOf('=')], def[(def.IndexOf('=') + 1)..]);
                        else
                            Tool.Printer.Warn($"Global vars `{args[ptr]}` early defined.");
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
                    // 未知命令
                    else
                    {
                        Tool.Printer.Err($"Unknown option:{args[ptr]}");
                        return 1;
                    }

                    ptr++;
                }
            }
            catch (Exception err)
            {
                Tool.Printer.Err(err.ToString());
                return 1;
            }


            Stopwatch timer = new Stopwatch();

            timer.Start();
            Engine.Engine.Start();
            timer.Stop();

            Tool.Printer.Put($"Use time {timer.Elapsed:dd\\.hh\\:mm\\:ss\\:fffffff}");

            return 0;
        }
    }
}
