/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Print.cs
 * Content: Print Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Print", License = "GOSCPS", Version = 1)]
    public class Print
    {

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <returns></returns>
        [Api.Macro("Print")]
        public bool PrintMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Common(param);
            return true;
        }

        /// <summary>
        /// 打印信息
        /// 然后退出
        /// </summary>
        /// <returns></returns>
        [Api.Macro("ErrorPrint")]
        public bool ErrorPrintMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Error(param);
            return true;
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <returns></returns>
        [Api.Macro("GoodPrint")]
        public bool GoodPrintMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Good(param);
            return true;
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <returns></returns>
        [Api.Macro("WarnPrint")]
        public bool WarnPrintMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{");
            Printer.Warn(param);
            return true;
        }
    }
}
