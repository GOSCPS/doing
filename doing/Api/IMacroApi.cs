/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    MacroApi.cs
 * Content: MacroApi Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Api
{
    /// <summary>
    /// 扩展的宏接口
    /// </summary>
    public interface IMacroApi
    {
        /// <summary>
        /// 宏托管函数
        /// </summary>
        /// <param name="param">调用参数</param>
        /// <param name="interpreter">调用的解释器，null即为全局解释器</param>
        /// <returns>是否执行成功</returns>
        public delegate bool Macro(string param, Build.Interpreter.Interpreter interpreter);
    }
}
