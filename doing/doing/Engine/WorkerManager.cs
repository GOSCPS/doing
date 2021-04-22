//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Worker.cs
// Content: Worker Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Doing.Engine
{
    /// <summary>
    /// 工作组
    /// </summary>
    public class WorkerTeam
    {
        public ConcurrentQueue<WorkerManager> WorkList { get; } = new();

        /// <summary>
        /// 有错误是否继续执行
        /// </summary>
        public bool KeepGoOn { get; init; }

        public WorkerTeam(bool keepGoOn = false)
        {
            KeepGoOn = keepGoOn;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// 
        /// <returns>执行成功返回true，否则false/returns>
        public bool Execute()
        {
            bool ret = true;
            while (true)
            {
                if (WorkList.TryDequeue(out WorkerManager? worker))
                {
                    bool executeResult = worker.Run();
                    // 执行
                    if (!executeResult && !KeepGoOn)
                    {
                        return false;
                    }

                    if (!executeResult)
                    {
                        // 有错误
                        ret = false;
                    }
                }
                else break;
            }

            return ret;
        }
    }

    /// <summary>
    /// 工作引擎
    /// 负责执行Target等
    /// </summary>
    public class WorkerManager
    {
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int MaxThread { get; init; }

        /// <summary>
        /// 在有错误的情况下是否继续
        /// </summary>
        public bool KeppGoOn { get; init; }

        /// <summary>
        /// 任务队列
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentQueue
            <IExecutable> taskList = new();

        /// <summary>
        /// 完成列表
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentBag
            <string> finishList = new();

        /// <summary>
        /// 锁
        /// </summary>
        private readonly object locker = new();

        /// <summary>
        /// 线程列表
        /// </summary>
        private Thread[] threadList = Array.Empty<Thread>();

        /// <summary>
        /// 错误列表
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentBag
            <Exception> errList = new();

        public WorkerManager(int maxThread,bool keepGoOn = false)
        {
            if (maxThread <= 0)
                throw new ArgumentException("The max count is less or equal to zero!", nameof(maxThread));

            MaxThread = maxThread;
            KeppGoOn = keepGoOn;
        }

        /// <summary>
        /// 添加目标
        /// </summary>
        /// <param name="target"></param>
        public void AddTask(IExecutable target)
        {
            taskList.Enqueue(target);
        }

        /// <summary>
        /// 检查目标是否已经完成
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsFinish(string targetName)
        {
            foreach (var finished in finishList)
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
        public string[] GetFinishList()
        {
            return finishList.ToArray();
        }


        /// <summary>
        /// 刷新线程
        /// </summary>
        public void ReFlushThread()
        {
            lock (locker)
            {
                for (int ptr = 0; ptr != threadList.Length; ptr++)
                {
                    // 线程未null或者已死亡
                    // 重新启动
                    if (!((threadList[ptr] != null) && threadList[ptr].IsAlive))
                    {
                        threadList[ptr] = new Thread(WorkThreadMethod)
                        {
                            Name = "Worker Thread-" + ptr.ToString(),
                            CurrentCulture = Thread.CurrentThread.CurrentCulture,
                            CurrentUICulture = Thread.CurrentThread.CurrentUICulture
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
        private IExecutable? GetTask()
        {
            if (taskList.TryDequeue(out IExecutable? result))
            {

                while (true)
                {
                    // 有错误则不继续等待
                    if (!errList.IsEmpty && !KeppGoOn)
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
        private void WorkThreadMethod()
        {
            IExecutable? executer = null;

            try
            {
                while (true)
                {
                    executer = GetTask();

                    // null为无效target
                    // 退出
                    if (executer == null)
                        return;

                    // 错误
                    if (!executer.Execute(this))
                    {
                        throw new DException.RuntimeException("The executable execute fail down!");
                    }

                    // 添加到完成列表
                    finishList.Add(executer.Name);
                }
            }
            catch (Exception err)
            {
                errList.Add(err);
                Tool.Printer.NoFormatErrLine($"*** {Thread.CurrentThread.Name} Error!");

                if (executer != null)
                    Tool.Printer.ErrLine("*** Err at task `{0}` ", executer.Name);

                Tool.Printer.NoFormatErrLine(err.ToString());
            }
        }


        /// <summary>
        /// 运行
        /// </summary>
        /// <returns>成功返回true，否则false</returns>
        public bool Run()
        {
            // 初始化线程
            lock (locker)
                threadList = new Thread[MaxThread];

            ReFlushThread();

            // 监视线程
            while (true)
            {
                // 所有线程死亡则退出
                lock (locker)
                {
                    bool allDeath = true;

                    for (int ptr = 0; ptr < threadList.Length; ptr++)
                    {
                        if (threadList[ptr].IsAlive)
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
