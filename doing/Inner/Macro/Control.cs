/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Control.cs
 * Content: Control Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Control", License = "GOSCPS", Version = 1)]
    public class Control
    {
        /// <summary>
        /// 结束target执行
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Break")]
        public bool BreakMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Common(param);
            //修改程序计数器
            //到末尾
            interpreter.ProgramCounter
                = interpreter.Source.Codes.Length;
            return true;
        }

        /// <summary>
        /// 引发错误
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Error")]
        public bool ErrorMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Error(param);
            return false;
        }
    }
}
