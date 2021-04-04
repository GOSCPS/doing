/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Function.cs
 * Content: Function Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace Doing.Engine.Utility
{
    /// <summary>
    /// 函数
    /// </summary>
    public abstract class Function
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public virtual string Name { get { return ""; } }

        /// <summary>
        /// 调用函数
        /// </summary>
        /// 
        /// <param name="callerContext">调用者上下文</param>
        /// <param name="args">调用参数</param>
        /// 
        /// <returns>函数返回值</returns>
        public abstract Variable Execute(Context callerContext, Variable[] args);
    }
}
