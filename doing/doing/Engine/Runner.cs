using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Doing.Engine
{
    /// <summary>
    /// 执行器
    /// </summary>
    class Runner
    {
        /// <summary>
        /// 模块信息
        /// </summary>
        public static BuildModuleInfo[] Modules { get; set; } = Array.Empty<BuildModuleInfo>();

        /// <summary>
        /// Target信息
        /// </summary>
        public static ConcurrentDictionary<string, (BuildModuleInfo, Target)> TargetList { get; set;} = new();

        /// <summary>
        /// 目标
        /// </summary>
        public static ConcurrentQueue<Target> AimTargets { get; set; } = new ConcurrentQueue<Target>();

        /// <summary>
        /// 完成列表
        /// </summary>
        private static readonly List<string> finishList = new();

        /// <summary>
        /// 错误列表
        /// </summary>
        private static readonly ConcurrentStack<System.Exception> ErrList = new();

        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object locker = new();

        /// <summary>
        /// 开始执行
        /// </summary>
        /// 
        /// <returns>构建成功返回true，否则false</returns>
        public static bool StartRunner()
        {
            if (AimTargets.IsEmpty)
                throw new Exception.RuntimeException("No target to build!");

            Thread[] worker = new Thread[Program.ThreadCount];

            // 启动线程
            for(int ptr=0;ptr < worker.Length; ptr++)
            {
                worker[ptr] = new Thread(WorkingPeople) {
                    Name = $"Worker-{ptr}"
                };

                worker[ptr].Start();
            }

            // 等待
            foreach(var t in worker)
            {
                t.Join();
            }

            // 有错误
            // 构建失败
            if (!ErrList.IsEmpty)
            {
                return false;
            }
            else return true;
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// 
        /// <returns>如果返回null，则不能获取任务</returns>
        public static Target? GetTask()
        {
            // 错误则不再继续构建
            if (!ErrList.IsEmpty)
                return null;

            if (AimTargets.TryDequeue(out Target? result))
            {

                while (true)
                {
                    // 有错误则不继续等待
                    if (!ErrList.IsEmpty)
                        return null;

                    // 检查完成列表
                    bool IsFinish = true;

                    lock (locker)
                    {
                        // 检查前置是否完成
                        foreach (var dep in result.Deps)
                            if (!finishList.Contains(dep))
                                IsFinish = false;
                    }

                    // 前置依赖完成
                    if (IsFinish)
                        break;

                    // 休眠
                    Thread.Sleep(50);
                }

                return result;

            }
            else return null;
        }

        /// <summary>
        /// 工作线程
        /// </summary>
        public static void WorkingPeople()
        {
            Target? target = null;
            try
            {
                while (true)
                {
                    if (!ErrList.IsEmpty)
                        return;

                    target = GetTask();

                    if (target == null)
                        return;

                    // 执行
                    PowerShellEngine engine = PowerShellEngine.Create(Thread.CurrentThread.Name + "-" + target.Name);

                    if (!engine.ExecuteScript(target.SourceCode, target.StartLine, target.FileName))
                    {
                        throw new Exception.RuntimeException("The PowerShell has error!");
                    }

                    // 添加到完成列表
                    lock (locker)
                        finishList.Add(target.Name);
                    
                }
            }
            catch(System.Exception err)
            {
                ErrList.Push(err);
                Tool.Printer.ErrLine($"*** {Thread.CurrentThread.Name} Error!");

                if (target != null)
                    Tool.Printer.ErrLine($"At file {target.FileName} StartLines {target.StartLine}");

                Tool.Printer.ErrLine($"{err.ToString().Replace("{", "{{")}");
            }
        }
    }
}
