/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Emiter.cs
 * Content: Emiter Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;


namespace Doing.Engine
{
    /// <summary>
    /// Awesome C#!
    /// </summary>
    static class Emiter
    {
        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="tokens"></param>
        public static void Compile(Token[] tokens)
        {
            // 忽略空输入
            if (tokens == null || tokens.Length == 0)
                return;

            // 添加标准库
            Standard.Standard.AddStandard();

            ParsingUtility.TokenMake token = new ParsingUtility.TokenMake
            {
                Tokens = tokens
            };

            // 解析
            List<Utility.Target> targets = new List<Utility.Target>();
            List<string> targetList = new List<string>();

            while (true)
            {
                if (token.IsEnd())
                    break;

                // 解析Target
                else if (token.Current.type == TokenType.keyword_target)
                {
                    var t= ParsingUtility.ParsingTarget.Parsing_Target(token);

                    // 检测重复
                    if (targetList.Contains(t.name))
                    {
                        if (t.body != null)
                            throw new RuntimeException($"Target `{t.name}` was defined!", t.body);
                        else
                            throw new RuntimeException($"Target `{t.name}` was defined!");
                    }
                    else
                    {
                        targetList.Add(t.name);
                        targets.Add(t);
                    }

                }
                // 解析函数
                else if(token.Current.type == TokenType.keyword_function)
                {
                    var func = ParsingUtility.ParsingFunction.Parsing_function(token);

                    if (!Utility.Context.GlobalFunctionTable.TryAdd(func.Name, func))
                        throw new CompileException($"Function `{func.Name}` defined!");
                }

                else throw new CompileException("Unknown Token Begin.",token.Current);
            }


            // 获取构建目标
            List<Utility.Target> aims = new List<Utility.Target>();

            foreach(var name in Program.GlobalTargets)
            {
                Utility.Target? target = null;

                foreach(var t in targets)
                {
                    if(t.name == name)
                    {
                        target = t;
                        break;
                    }
                }

                // 没有获取目标
                // 警告
                if (target == null)
                    throw new RuntimeException($"Miss aims `{name}`!");

                else
                {
                    aims.Add(target);
                }
            }

            // 没有设置目标
            // 警告 退出
            if(aims.Count == 0)
            {
                Tool.Printer.WarnLine("Warn:Not target to do!");
                return;
            }

            // 排序
            foreach(var t in Algorithm.Topological.Sort(aims.ToArray(), targets.ToArray()))
            {
                totalTargets.Enqueue(t);
            }

            // 多线程执行
            List<Thread> builder = new List<Thread>();
            for(uint a=0;a < Program.ThreadCount;a++)
            {
                Thread thread = new Thread(TaskThread)
                {
                    Name = $"Doing Build Thread {a}"
                };
                thread.Start();
                builder.Add(thread);
            }

            // 等待执行完毕
            foreach(var t in builder)
            {
                t.Join();
            }

            if (!exceptions.IsEmpty)
                throw new RuntimeException("Builder Has exceptions!");
        }

        // target列表
        private static readonly ConcurrentQueue<Utility.Target> totalTargets 
            = new ConcurrentQueue<Utility.Target>();

        // 异常列表
        private static readonly ConcurrentBag<(Exception, Utility.Target?)> exceptions 
            = new ConcurrentBag<(Exception, Utility.Target?)>();

        // 完成列表
        private static readonly ConcurrentBag<string> complete 
            = new ConcurrentBag<string>();

        // 获取任务
        private static Utility.Target? GetTask()
        {
            if (totalTargets.TryDequeue(out Utility.Target? target))
            {
                // 检查依赖
                while (true)
                {
                    bool depBuilt = true;
                    foreach (var v in target.deps)
                    {
                        if (!complete.Contains(v))
                            depBuilt = false;
                    }

                    // 依赖已经构建完毕则返回
                    if (depBuilt)
                        return target!;

                    // 没有终止构建的错误发生
                    else if (!exceptions.IsEmpty)
                        return null;

                    // 则等待依赖构建完成
                    else Thread.Sleep(200);
                }
            }
            else return null;
        }

        // 任务线程
        private static void TaskThread()
        {
            Utility.Target? target = null;
            try
            {
                while (true)
                {
                    // 不在错误环境下执行
                    if (!exceptions.IsEmpty)
                        return;

                    target = GetTask();

                    if (target == null)
                        return;

                    else target.body.SafeExecute(new Utility.Context());

                    // 添加到完成列表
                    complete.Add(target.name);
                }
            }
            catch(Exception err)
            {
                exceptions.Add((err, target));

                // 错误处理
                Tool.Printer.ErrLine($"*** {Thread.CurrentThread.Name} Error! ***");

                if (target != null)
                    Tool.Printer.ErrLine($"At Target `{target.name}`!");

                Tool.Printer.ErrLine(err.ToString().Replace("{", "{{").Replace("}", "}}"));
            }
        }
    }
}
