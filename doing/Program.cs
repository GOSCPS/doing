/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Program.cs
 * Content: Program Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace doing
{
    /// <summary>
    /// Doing主程序
    /// </summary>
    class Program
    {
        /// <summary>
        /// Doing版本号
        /// </summary>
        public static readonly string DoingVersion =
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 所有异常Target的返回值
        /// </summary>
        public static ConcurrentStack<ValueTuple<TaskInfo, Exception>> 
            ErrorReturn = new ConcurrentStack<ValueTuple<TaskInfo, Exception>>();

        /// <summary>
        /// 全局变量表
        /// </summary>
        public static ConcurrentDictionary<string, string> GolbalVariable
            = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 全局Rule
        /// </summary>
        public static ConcurrentDictionary<string,Rule> GolbalRules = 
            new ConcurrentDictionary<string, Rule>();

        /// <summary>
        /// 全局Target
        /// </summary>
        public static ConcurrentDictionary<string, Target> GolbalTargets = 
            new ConcurrentDictionary<string, Target>();

        /// <summary>
        /// Main入口函数
        /// </summary>
        /// <param name="args">命令行参数</param>
        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("=====Doing======");
                Console.WriteLine("=Made By GOSCPS=");
                Console.WriteLine($"=Version{DoingVersion}=");
                Console.WriteLine("================");

                //线程池设置
                //默认堆栈大小
                ThreadPool.ThreadPool.StackSize = 0;
                ThreadPool.ThreadPool.ThreadMaxCount = Environment.ProcessorCount;

                HashSet<string> targets = new HashSet<string>();

                //解析命令行参数
                for (int a = 0; a < args.Length; a++)
                {
                    if (args[a] == "-help")
                    {
                        if (args.Length >= 2)
                        {
                            Console.WriteLine("Warn:Param `-help` will ignore other params");
                        }
                        Console.WriteLine("The build system `doing` can build everything!");
                        Console.WriteLine("Usgae:doing [options]");
                        Console.WriteLine("options:");
                        Console.WriteLine("\t-help Get Help");
                        Console.WriteLine("\t-version Get doing version");
                        Console.WriteLine("\t-S Key Value:define build key-value`");
                        Console.WriteLine("\t-D[KEY]:define build variable");
                        Console.WriteLine("\tno options:build default `build.doing`");
                        return 0;
                    }
                    else if (args[a] == "-version")
                    {
                        Console.WriteLine("The build system `doing` can build everything!");
                        if (args.Length >= 2)
                        {
                            Console.WriteLine("Warn:Param `-help` will ignore other params");
                        }
                        Console.WriteLine($"doing {DoingVersion} in {Environment.OSVersion.Platform}");
                        return 0;
                    }

                    //设置
                    else if (args[a] == "-S")
                    {
                        if ((a + 2) < args.Length)
                        {
                            GolbalVariable[args[a + 1]] = args[a + 2];
                            a += 2;
                        }
                        else
                        {
                            Console.Error.WriteLine("Error:options -S no right!");
                            return -1;
                        }
                    }

                    //定义
                    else if (args[a] == "-D")
                    {
                        if (++a < args.Length)
                        {
                            GolbalVariable[args[a]] = "";
                        }
                        else
                        {
                            Console.Error.WriteLine("Error:options -D no right!");
                            return -1;
                        }
                    }

                    else
                    {
                        targets.Add(args[a]);
                    }
                }

                ThreadPool.ThreadPool.StartThreadPoolManager();
                Console.WriteLine($"Will build targets:\n\t{string.Join("\n\t", targets)}");

                var info = ReadDoingFile("build.doing");

                if(info == null)
                {
                    Console.Error.WriteLine("Error:Parse Json File Error!");
                    return -1;
                }

                foreach (var r in info.Rules)
                {
                    GolbalRules.GetOrAdd(r.Name, r);
                }

                foreach (var t in info.Targets)
                {
                    GolbalTargets.GetOrAdd(t.Name, t);
                }

                //执行
                foreach (var a in targets)
                {
                    if (GolbalTargets.TryGetValue(a, out Target value))
                    {
                        IRunner runner = new NativeRunner();
                        runner.target = value;
                        ThreadPool.ThreadPool.AddTarget(runner);
                    }
                    else
                    {
                        Console.WriteLine($"Error:Target `{a}` not found");
                        ErrorReturn.Push((new TaskInfo()
                        {
                            ThreadName = "Main",
                            Error = null,
                            ThreadId = Thread.CurrentThread.ManagedThreadId,
                            RunTarget = null
                        }, new Exception($"Error:Target `{a}` not found")));
                        break;
                    }
                }

                ThreadPool.ThreadPool threadPool = new ThreadPool.ThreadPool();

                //检查是否全部完成
                bool AllFinally = false;
                while (true) {
                    Thread.Sleep(200);
                    if (AllFinally)
                    {
                        break;
                    }

                    AllFinally = true;
                    foreach (var a in targets)
                    {
                        if (!threadPool[a])
                        {
                            AllFinally = false;
                        }
                    }
                }

            }
            catch (Exception err)
            {
                Console.Error.WriteLine("Error:Build failure");
                Console.Error.WriteLine(err.ToString());
                return -1;
            }
            finally
            {
                ThreadPool.ThreadPool.EndThreadPoolManager();

                //打印错误堆栈
                if(!ErrorReturn.IsEmpty)
                {
                    Console.Error.WriteLine("Error:Build fail down");
                    foreach(var a in ErrorReturn)
                    {
                        Console.Error.WriteLine($"Error Build in {a.Item1.ThreadName}");
                        if(a.Item2 != null)
                        {
                            Console.Error.WriteLine(a.Item2.ToString());
                        }
                        else
                        {
                            Console.Error.WriteLine("No Exception");
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>读取到的文件名称</returns>
        public static BuildFile ReadDoingFile(string name)
        {
            BuildFile build =
                JsonSerializer.Deserialize<BuildFile>(File.ReadAllText(name, System.Text.Encoding.UTF8));

            List<Target> ts = new List<Target>();
            List<Rule> rs = new List<Rule>();

            ts.AddRange(build.Targets);
            rs.AddRange(build.Rules);

            if (build == null)
            {
                return null;
            }
            else
            {
                foreach (var file in build.Include)
                {
                    BuildFile include = ReadDoingFile(file);
                    ts.AddRange(include.Targets);
                    rs.AddRange(include.Rules);
                }
            }

            //检查重复项
            HashSet<Target> CheckTarget = new HashSet<Target>();
            HashSet<Rule> CheckRule = new HashSet<Rule>();

            foreach(var r in rs)
            {
                if (CheckRule.Contains(r))
                {
                    Console.Error.WriteLine($"Warn:repeat rule `{r.Name}` in file:{name}");
                    return null;
                }
                else
                {
                    CheckRule.Add(r);
                }
            }
            foreach (var t in ts)
            {
                if (CheckTarget.Contains(t))
                {
                    Console.Error.WriteLine($"Warn:repeat target `{t.Name}` in file:{name}");
                    return null;
                }
                else
                {
                    CheckTarget.Add(t);
                }
            }

            return build;
        }
    }
}
