/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ThreadPool.cs
 * Content: ThreadPool Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace doing.ThreadPool
{
    /// <summary>
    /// Doing线程池
    /// </summary>
    class ThreadPool
    {
        //线程最大数量
        public static int ThreadMaxCount { get; set; }

        //堆栈大小
        public static int StackSize { get; set; }

        /// <summary>
        /// 检查任务是否完成
        /// </summary>
        /// <param name="threadName">添加Target时的name</param>
        /// <returns>完成返回true，否则false</returns>
        public bool this[string threadName]
        {
            get
            {
                lock (locker)
                {
                    //不在线程池 or 等待池
                    //任务完成
                    foreach(var a in taskList)
                    {
                        if(threadName == a.Item2)
                        {
                            return false;
                        }
                    }
                    foreach(var a in threadList)
                    {
                        if(threadName == a.Name)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        //任务列表
        private static List<ValueTuple<IRunner, string>> taskList
            = new List<ValueTuple<IRunner, string>>();

        //线程列表
        private static List<Thread> threadList = new List<Thread>();

        //锁
        private static object locker = new object();

        //线程池管理线程运行
        private static bool threadPoolRun = false;
        private static object __threadPoolRunLocker = new object();

        /// <summary>
        /// 添加线程池任务
        /// </summary>
        /// <param name="runner">任务函数</param>
        public static void AddTarget(
            IRunner runner)
        {
            lock (locker)
            {
                //清除已死线程
                foreach (var obj in threadList)
                {
                    if (!obj.IsAlive)
                    {
                        threadList.Remove(obj);
                    }
                }

                //已有重复任务，不再添加
                //检查任务池和线程池
                //检查仅限于：Target名称
                foreach(var a in taskList)
                {
                    if(a.Item1.target.Name == runner.target.Name)
                    {
                        return;
                    }
                }
                foreach (var a in threadList)
                {
                    if (a.Name == runner.target.Name)
                    {
                        return;
                    }
                }


                //线程已满，插入任务队列
                if (threadList.Count >= ThreadMaxCount)
                {
                    taskList.Add((runner, runner.target.Name));
                    return;
                }


                //线程未满，执行
                Thread t = new Thread(runner.Run, StackSize);
                t.Name = runner.target.Name;
                t.Start();
                threadList.Add(t);
            }
        }

        /// <summary>
        /// 开启线程池管理程序
        /// </summary>
        public static void StartThreadPoolManager()
        {
            lock(__threadPoolRunLocker)
                if (threadPoolRun)
                    return;

            lock(__threadPoolRunLocker)
                threadPoolRun = true;
            Thread thread = new Thread(ThreadPoolManager);
            thread.Start();
        }

        /// <summary>
        /// 关闭线程池管理程序
        /// </summary>
        public static void EndThreadPoolManager()
        {
            lock(__threadPoolRunLocker)
                threadPoolRun = false;
        }

        /// <summary>
        /// 线程管理器管理线程
        /// </summary>
        private static void ThreadPoolManager()
        {
            try
            {
                while (true)
                {
                    lock (__threadPoolRunLocker)
                        if (!threadPoolRun)
                            return;

                    //休眠100ms
                    Thread.Sleep(200);

                    //管理线程
                    lock (locker)
                    {
                        
                        //清除已死线程
                        for(int a=0;a < threadList.Count;a++)
                        {
                            if (!threadList[a].IsAlive)
                            {
                                threadList.RemoveAt(a);
                            }
                        }

                        //线程未满 && 有任务
                        //添加任务
                        if(threadList.Count < ThreadMaxCount
                            && taskList.Count != 0)
                        {
                            var task = taskList[0];
                            taskList.RemoveAt(0);

                            Thread thread = new Thread(task.Item1.Run, StackSize)
                            {
                                Name = task.Item2
                            };
                            thread.Start();

                            threadList.Add(thread);
                        }
                    }
                }
            }
            catch(Exception err)
            {
                Console.WriteLine("Fatal Error:ThreadPoolManager Thread Error!");
                Console.WriteLine("This accident shouldn't happen!");
                Console.WriteLine(err.ToString());
                return;
            }
        }
    }
}
