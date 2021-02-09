/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    BuildController.cs
 * Content: BuildController Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Threading;

namespace doing.Build
{
    /// <summary>
    /// 构建控制器
    /// </summary>
    public static class BuildController
    {

        /// <summary>
        /// 线程列表
        /// </summary>
        private static List<Thread> ThreadList = new List<Thread>();

        /// <summary>
        /// 等待的Target队列
        /// </summary>
        private static Queue<Target> WaitTargetQueue = new Queue<Target>();

        /// <summary>
        /// 完成的Target列表
        /// </summary>
        private static List<string> FinishedTarget = new List<string>();

        /// <summary>
        /// 锁
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// 构建是否在正确的状态
        /// </summary>
        private static bool Right = true;

        /// <summary>
        /// 错误列表
        /// </summary>
        private static List<System.Exception> ErrorList = new List<System.Exception>();

        /// <summary>
        /// 根据已有GlobalContext构建
        /// </summary>
        public static void Build()
        {
            //完善目标
            foreach (var str in GlobalContext.AimTargetStrs)
            {
                bool got = false;
                foreach (var tar in GlobalContext.TargetList)
                {
                    if (tar.Name == str)
                    {
                        got = true;
                        GlobalContext.AimTarget.Add(tar);
                    }
                }
                if (!got)
                {
                    throw new System.Exception("Not found aim targets");
                }
            }
            //排序
            var sortedAimTargets = Algorithm.TopSort.Sort();

            //入队
            foreach (var t in sortedAimTargets)
            {
                WaitTargetQueue.Enqueue(t);
            }

            //线程循环开启
            while (true)
            {
                //清理已死线程
                for (int a = 0; a < ThreadList.Count; a++)
                {
                    if (!ThreadList[a].IsAlive)
                    {
                        ThreadList.RemoveAt(a);
                        //Remove后当前位置被下一个Thread填充
                        //抵消++防止错过当前Thread
                        //a--;
                    }
                }


                lock (locker)
                {
                    //正确才继续构建
                    if (Right)
                        //线程未满 & 有空余任务：分配任务
                        while (WaitTargetQueue.Count != 0)
                        {
                            if (ThreadList.Count < GlobalContext.MaxThreadCount)
                            {
                                Target t = WaitTargetQueue.Dequeue();
                                Thread thread = new Thread(new ParameterizedThreadStart(BuildThread))
                                {
                                    Name = "$Work thread for {t.Name}"
                                };
                                thread.Start(t);
                                ThreadList.Add(thread);
                            }
                        }
                }

                lock (locker)
                    if (ThreadList.Count == 0 && WaitTargetQueue.Count == 0)
                        break;

                //降低占用
                Thread.Sleep(200);
            }

            //事后检查
            if ((!Right) || ErrorList.Count != 0)
            {
                Printer.Error("Doing build fail!");

                //炸Error时应该已经Print
                /*foreach(var err in ErrorList)
                {
                    Printer.Error(err.ToString());
                }*/
                Environment.Exit(-1);
            }
            else
            {
                Printer.Good("Doing build successful!");
                Environment.Exit(0);
            }
            return;
        }

        /// <summary>
        /// 构建线程
        /// </summary>
        /// <param name="target">构建的target</param>
        public static void BuildThread(object target)
        {
            try
            {
                bool depFinished = true;
                //等待依赖完成
                while (true)
                {
                    lock (locker)
                        foreach (var dep in ((Target)target).Deps)
                        {
                            if (!FinishedTarget.Contains(dep.Name))
                                depFinished = false;
                        }

                    if (depFinished)
                    {
                        break;
                    }
                    else
                    {
                        //降低占用
                        Thread.Sleep(200);

                        //已经失败则取消
                        lock (locker)
                            if (!Right)
                                return;

                        //重新尝试
                        depFinished = true;
                    }
                }

                Interpreter.Interpreter interpreter = new Interpreter.Interpreter();
                interpreter.RunTarget((Target)target);

                //添加到完成列表
                lock (locker)
                    FinishedTarget.Add(((Target)target).Name);
            }
            catch (System.Exception err)
            {
                lock (locker)
                {
                    Right = false;
                    ErrorList.Add(err);
                }
                Printer.Error(err.ToString());
            }
        }

    }
}
