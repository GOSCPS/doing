/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    MacroAttribute.cs
 * Content: MacroAttribute Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace doing.Api
{


    /// <summary>
    /// 声明宏
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MacroAttribute : Attribute
    {
        /// <summary>
        /// 宏名称
        /// </summary>
        public string MacroName { init; get; }

        public MacroAttribute(string name)
        {
            MacroName = name;
        }
    }
}
