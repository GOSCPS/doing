//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Worker.cs
// Content: Worker Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Doing.Engine
{
    /// <summary>
    /// 工作引擎
    /// </summary>
    public static class Worker
    {
        /// <summary>
        /// 任务队列
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentQueue
            <Target> taskList = new();

        /// <summary>
        /// 完成列表
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentBag
            <string> finishList = new();

        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object locker = new();

        /// <summary>
        /// 线程列表
        /// </summary>
        private static Thread[] threadList = Array.Empty<Thread>();

        /// <summary>
        /// 错误列表
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentBag
            <Exception> errList = new();

        /// <summary>
        /// 添加目标
        /// </summary>
        /// <param name="target"></param>
        public static void AddTarget(Target target)
        {
            taskList.Enqueue(target);
        }

        /// <summary>
        /// 检查目标是否已经完成
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsFinish(string targetName)
        {
            foreach(var finished in finishList)
            {
                if (targetName == finished)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取已完成目标无序列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetFinishList()
        {
            return finishList.ToArray();
        }


        /// <summary>
        /// 刷新线程
        /// </summary>
        public static void ReFlushThread()
        {
            lock (locker)
            {
                for(int ptr=0;ptr != threadList.Length; ptr++)
                {
                    // 线程未null或者已死亡
                    // 重新启动
                    if(!((threadList[ptr] != null) && threadList[ptr].IsAlive))
                    {
                        threadList[ptr] = new Thread(WorkThreadMethod)
                        {
                            Name = "Worker Thread-" + ptr.ToString(),
                        };
                        threadList[ptr].Start();
                    }
                }

            }
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// 
        /// <returns>如果返回null，则不能获取任务</returns>
        private static Target? GetTask()
        {
            // 错误则不再继续构建
            if (!errList.IsEmpty)
                return null;

            if (taskList.TryDequeue(out Target? result))
            {

                while (true)
                {
                    // 有错误则不继续等待
                    if (!errList.IsEmpty)
                        return null;

                    // 检查前置是否完成
                    bool allFinish = true;

                    foreach (var dep in result.Deps)
                        if (!IsFinish(dep))
                            allFinish = false;

                    // 前置依赖完成
                    if (allFinish)
                        break;

                    // 未完成，等一会
                    Thread.Sleep(50);
                }

                return result;

            }
            else return null;
        }

        /// <summary>
        /// 工作线程
        /// </summary>
        private static void WorkThreadMethod()
        {
            Target? target = null;

            try
            {
                while (true)
                {
                    // 有错误则不再继续
                    if (!errList.IsEmpty)
                        return;

                    target = GetTask();

                    // null为无效target
                    // 视为错误，退出
                    if (target == null)
                        return;

                    target.Execute();

                    // 添加到完成列表
                    finishList.Add(target.Name);
                }
            }
            catch (Exception err)
            {
                errList.Add(err);
                Tool.Printer.NoFormatErrLine($"*** {Thread.CurrentThread.Name} Error!");

                if (target != null)
                    Tool.Printer.NoFormatErrLine($"*** In target `{target.Name}` At {target.DefineLine.Position.SourceFile.FullName} " +
                        $"StartLines {target.DefineLine.LineNumber}");

                Tool.Printer.NoFormatErrLine(err.ToString());
            }
        }


        /// <summary>
        /// 运行
        /// </summary>
        /// <returns>成功返回true，否则false</returns>
        public static bool Run()
        {
            // 初始化线程
            lock (locker)
                threadList = new Thread[Program.ThreadCount];

            ReFlushThread();

            // 监视线程
            while (true)
            {
                // 所有线程死亡则退出
                lock (locker)
                {
                    bool allDeath = true;

                    for(int ptr=0;ptr < threadList.Length; ptr++)
                    {
                        if(threadList[ptr].IsAlive)
                        {
                            allDeath = false;
                            break;
                        }
                    }

                    if (allDeath)
                        break;
                }
                
                // 降低资源占用
                Thread.Sleep(100);
            }

            // 检索错误列表
            if (errList.IsEmpty)
                return true;
            else return false;
        }

    }
}
