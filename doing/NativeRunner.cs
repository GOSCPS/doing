/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    NativeRunner.cs
 * Content: NativeRunner Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace doing
{
    /// <summary>
    /// 本机运行
    /// </summary>
    class NativeRunner : IRunner
    {
        public Target target { get; set; }

        private ValueTuple<string,string> ParseRun(Run run)
        {
            if (Program.GolbalRules.TryGetValue(run.Rule, out Rule value))
            {
                return (value.Proc, value.Args);
            }
            else
            {
                throw new Exception($"Doing Error:Can't find the rule `{run.Rule}`");
            }
        }

        public void Run()
        {
            try
            {
                if(!Program.ErrorReturn.IsEmpty)
                {
                    Console.Error.WriteLine($"Can't complete {target.Name}.Because An error occurred.");
                    return;
                }
                if(target == null)
                {
                    throw new Exception($"Doing Error:Try to build null target!");
                }
                if(target.Name == "doing")
                {
                    Console.WriteLine("The build system `doing` can build itself!");
                }

                //执行依赖
                foreach(var dep in target.Depend)
                {
                    if(Program.GolbalTargets.TryGetValue(dep,out Target target))
                    {
                        IRunner runner = new NativeRunner();
                        runner.target = target;
                        ThreadPool.ThreadPool.AddTarget(runner);
                    }
                    else
                    {
                        throw new Exception($"Doing Error:Can't find depend `{dep}` of `{target.Name}`");
                    }
                }

                //挨个执行
                foreach(var cmd in target.RunList)
                {
                    var command = ParseRun(cmd);

                    var proc = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = command.Item1,
                            Arguments = command.Item2,
                            //使用系统shell启动
                            UseShellExecute = false,
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            RedirectStandardInput = false,
                            CreateNoWindow = false
                        }
                    };

                    foreach(var ev in cmd.With)
                    {
                        proc.StartInfo.EnvironmentVariables.Add(ev.Key, ev.Value);
                    }

                    if (!proc.Start())
                    {
                        throw new Exception("Doing Error:Process started fell failure");
                    }
                    else
                    {
                        proc.WaitForExit();
                        int result = proc.ExitCode;
                        if(result != 0)
                        {
                            throw new Exception($"Doing Error:Process return `{result}`");
                        }
                    }

                }
            }
            catch(Exception err)
            {
                TaskInfo taskInfo = new TaskInfo()
                {
                    Error = err,
                    RunTarget = target,
                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                    ThreadName = Thread.CurrentThread.Name
                };

                Program.ErrorReturn.Push((taskInfo, err));
                return;
            }
        }
    }
}
