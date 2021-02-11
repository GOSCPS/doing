/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Print.cs
 * Content: Print Source Files
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
    /// 打印机
    /// </summary>
    [Api.DoingExpand("doing-InnerExpand.Macro.Print", License = "GOSCPS", Version = 1)]
    public class Print
    {

        [Api.Macro("Print")]
        public bool PrintMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            Printer.Common(param.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }


        [Api.Macro("PrintError")]
        public bool PrintErrorMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            Printer.Error(param.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("PrintWarn")]
        public bool PrintWarnMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            Printer.Warn(param.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("PrintGood")]
        public bool PrintGoodMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            Printer.Good(param.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("Printv")]
        public bool PrintvMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if(!MacroTool.GetStringFromString(param,interpreter,out string result))
            {
                Printer.Error($"PrintvMacro Error:Can't found parse `{param.Replace("{", "{{")}`");
                return false;
            }
            Printer.Common(result.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("PrintvError")]
        public bool PrintvErrorMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!MacroTool.GetStringFromString(param, interpreter, out string result))
            {
                Printer.Error($"PrintvErrorMacro Error:Can't found parse `{param.Replace("{", "{{")}`");
                return false;
            }
            Printer.Error(result.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("PrintvWarn")]
        public bool PrintvWarnMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!MacroTool.GetStringFromString(param, interpreter, out string result))
            {
                Printer.Error($"PrintvWarnMacro Error:Can't found parse `{param.Replace("{", "{{")}`");
                return false;
            }
            Printer.Warn(result.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }

        [Api.Macro("PrintvGood")]
        public bool PrintvGoodMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!MacroTool.GetStringFromString(param, interpreter, out string result))
            {
                Printer.Error($"PrintvGoodMacro Error:Can't found parse `{param.Replace("{", "{{")}`");
                return false;
            }
            Printer.Good(result.Replace("{", "{{").Replace("}", "}}"));
            return true;
        }
    }
}
