/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Os.cs
 * Content: Os Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doing.Inner.Macro
{
    /// <summary>
    /// 提供一些操作系统交互
    /// </summary>
    [Api.DoingExpand("doing-InnerExpand.Macro.Os", License = "GOSCPS", Version = 1)]
    public class Os
    {

        /// <summary>
        /// 安全的设置系统环境变量
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("AddToPath")]
        public bool AddToPathMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if(!MacroTool.GetStringFromString(param,interpreter,out string result))
            {
                Printer.Error($"AddToPathMacro Error:Can't found var `{param}`");
                return false;
            }

            Environment.SetEnvironmentVariable("PATH", 
                Environment.GetEnvironmentVariable("PATH") + ";" + result);

            return true;
        }



    }
}
