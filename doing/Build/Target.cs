/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Target.cs
 * Content: Target Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace doing.Build
{
    /// <summary>
    /// 目标
    /// </summary>
    public class Target
    {
        /// <summary>
        /// 目标名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 目标依赖
        /// </summary>
        public Target[] Deps { get; set; }

        /// <summary>
        /// 目标代码
        /// </summary>
        public ValueTuple<string, Interpreter.LineInfo>[] Codes { get; set; }
    }
}
