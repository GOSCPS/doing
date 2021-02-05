/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    IRunner.cs
 * Content: IRunner Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Text;

namespace doing
{
    /// <summary>
    /// 每个执行者负责执行命令
    /// </summary>
    interface IRunner
    {
        /// <summary>
        /// 执行目标
        /// </summary>
        /// <param name="target">要执行的目标</param>
        void Run();

        /// <summary>
        /// 任务
        /// </summary>
        Target target { get; set; }
    }
}
