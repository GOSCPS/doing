/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    GlobalContext.cs
 * Content: GlobalContext Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System.Collections.Generic;

namespace doing.Build
{
    /// <summary>
    /// 全局上下文
    /// </summary>
    static class GlobalContext
    {
        /// <summary>
        /// 多线程修改需要加锁
        /// </summary>
        public static readonly object GlobalContextLocker = new object();

        /// <summary>
        /// 要构建的文件的名称
        /// </summary>
        public static string FileName = "";

        /// <summary>
        /// 全局环境变量
        /// </summary>
        public static readonly IDictionary<string, string> GlobalEnvironmentVariables
            = new Dictionary<string, string>();

        /// <summary>
        /// 要构建的目标的名称
        /// </summary>
        public static readonly IList<string> AimTargetStrs = new List<string>();

        /// <summary>
        /// 要构建的目标
        /// </summary>
        public static readonly IList<Target> AimTarget = new List<Target>();

        /// <summary>
        /// 源文件
        /// </summary>
        public static string[] Source { get; set; }

        /// <summary>
        /// 处理过后的Target列表
        /// </summary>
        public static Target[] TargetList { get; set; }

        /// <summary>
        /// 最大线程数量
        /// </summary>
        public static int MaxThreadCount { get; set; }
    }
}
