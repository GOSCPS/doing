/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Program.cs
 * Content: Program Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace doing
{
    /// <summary>
    /// 命令行参数
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// 定义的全局变量
        /// </summary>
        [Option('D', "Define", HelpText = "Define global environment variables.")]
        public IList<string> Defines { get; set; }

        /// <summary>
        /// 要构建的Target
        /// </summary>
        [Value(0, HelpText = "What target you want to build.")]
        public IList<string> AimTargets { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Option('F', "File", HelpText = "Set the doing file.Default is `build.doing`.", Default = "build.doing")]
        public string FileName { get; set; }

        /// <summary>
        /// 定义的全局变量
        /// </summary>
        [Option('T', "Thread", HelpText = "Define max thread count you want to use.", Default = -1)]
        public int ThreadCount { get; set; }
    }


    public class Program
    {
        /// <summary>
        /// Doing版本号
        /// </summary>
        public static string DoingVersion
        {
            get { return ((Assembly.GetEntryAssembly()).GetName()).Version.Major.ToString(); }
        }


        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>返回值</returns>
        static int Main(string[] args)
        {
            try
            {
                Printer.Common("===== doing =====");
                Printer.Common("== from GOSCPS ==");
                Printer.Common($"===== ver {DoingVersion} =====");
                Printer.Common("=================");

                //解析命令行参数
                bool parseArgumentsSuccess = true;

                var result = Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        //-D Key Value
                        if ((options.Defines.Count) % 2 != 0)
                        {
                            Printer.Error("Doing Error:Define need value.");
                            parseArgumentsSuccess = false;
                            return;
                        }
                        for (int a = 0; a < (options.Defines.Count / 2); a += 2)
                        {
                            Build.GlobalContext.GlobalEnvironmentVariables
                            .Add(options.Defines[a], options.Defines[a + 1]);
                        }

                        //构建目标
                        foreach (var target in options.AimTargets)
                            Build.GlobalContext.AimTargetStrs.Add(target);

                        //构建文件名称
                        Build.GlobalContext.FileName = options.FileName;

                        //设置线程数量
                        if (options.ThreadCount > 0)
                        {
                            Build.GlobalContext.MaxThreadCount
                            = options.ThreadCount;
                        }
                        else
                        {
                            Build.GlobalContext.MaxThreadCount
                            = Environment.ProcessorCount;
                        }

                    })
                    .WithNotParsed(errors =>
                    {
                        parseArgumentsSuccess = false;
                        Printer.Error("Doing Error:Parse arguments fail down or no arguments.");
                    });

                if (!parseArgumentsSuccess)
                {
                    return -1;
                }

                //默认构建default
                if (Build.GlobalContext.AimTargetStrs.Count == 0)
                {
                    Printer.Warn("Doing Warn:Not found target in arguments.Default build `default`.");
                    Build.GlobalContext.AimTargetStrs.Add("default");
                }

                //读取文件
                string[] lines;
                try
                {
                    lines = File.ReadAllLines(Build.GlobalContext.FileName, Encoding.UTF8);
                }
                catch (IOException err)
                {
                    Printer.Error("Doing Error:IO Exception!");
                    Printer.Error(err.ToString());
                    return -1;
                }

                //加载自己
                Expand.ExpandManager.LoadExpandFromFile(
                    typeof(Program).Assembly.Location);

                //添加操作系统定义
                Build.GlobalContext.GlobalEnvironmentVariables.Add(
                    "__OS__"
                    , Environment.OSVersion.Platform.ToString());

                //编译
                Build.GlobalContext.Source = lines;
                Build.Interpreter.Interpreter.Run();

                //构建
                Build.BuildController.Build();
            }
            catch (System.Exception err)
            {
                Printer.Error("Doing build failed.");
                Printer.Error(err.ToString());
            }
            return -1;
        }
    }
}