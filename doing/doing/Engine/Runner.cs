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
    public class Runner
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
        /// 所有存在的Targets
        /// </summary>
        public static List<Target> AllExistTargets = new();

        /// <summary>
        /// 队列目标
        /// 动态减少
        /// </summary>
        public static ConcurrentQueue<Target> AimTargets { get; set; } = new ConcurrentQueue<Target>();

        /// <summary>
        /// 所有目标
        /// </summary>
        public static Target[] TotalBuildTargets { get; set; } = Array.Empty<Target>();

        /// <summary>
        /// 错误列表
        /// </summary>
        private static readonly ConcurrentStack<System.Exception> ErrList = new();

        /// <summary>
        /// 全局变量表
        /// </summary>
        public static ConcurrentDictionary<string, object> GlobalDoingVariableTable { get; } = new();

        /// <summary>
        /// 完成列表
        /// </summary>
        private static readonly List<string> finishList = new();

        /// <summary>
        /// 线程表
        /// </summary>
        private static Thread[] threads = Array.Empty<Thread>();

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object threadsLocker = new();

        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object finishListLocker = new();

        /// <summary>
        /// 检查Target是否已经构建完成
        /// </summary>
        /// <returns></returns>
        public static bool CheckTargetFinish(string targetName)
        {
            lock (finishListLocker)
            {
                return finishList.Contains(targetName);
            }
        }

        /// <summary>
        /// 重新启动所有工作线程
        /// </summary>
        /// <returns></returns>
        public static void ReStartThreads()
        {
            lock (threadsLocker)
            {
                for(int ptr=0;ptr < threads.Length; ptr++)
                {
                    if(threads[ptr] == null || (!threads[ptr].IsAlive))
                    {
                        threads[ptr] = new Thread(WorkingPeople)
                        {
                            Name = $"Worker Thread-{ptr}"
                        };

                        threads[ptr].Start();
                    }
                }
            }
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// 
        /// <returns>构建成功返回true，否则false</returns>
        public static bool StartRunner()
        {
            if (AimTargets.IsEmpty)
                throw new Exception.RuntimeException("No target to build!");

            lock (threadsLocker)
            {
                threads = new Thread[Program.ThreadCount];
            }

            // 初始化线程
            ReStartThreads();

            // 监察
            while (true)
            {
                lock (threadsLocker)
                {
                    // 检查线程是否都在运行
                    bool end = true;

                    foreach (var t in threads)
                    {
                        if (t.IsAlive)
                        {
                            end = false;
                            break;
                        }
                    }

                    if (end)
                        break;
                }
                Thread.Sleep(50);
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

                    lock (finishListLocker)
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
                    lock (finishListLocker)
                        finishList.Add(target.Name);
                    
                }
            }
            catch(System.Exception err)
            {
                ErrList.Push(err);
                Tool.Printer.ErrLine($"*** {Thread.CurrentThread.Name} Error!");

                if (target != null)
                    Tool.Printer.ErrLine($"*** At file {target.FileName} StartLines {target.StartLine}");

                Tool.Printer.ErrLine(err.ToString().Replace("{", "{{").Replace("}","}}"));
            }
        }
    }
}
