/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Def.cs
 * Content: Def Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Def", License = "GOSCPS", Version = 1)]
    public class Def
    {


        [Api.Macro("Def")]
        public bool DefMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains('='))
            {
                Printer.Error("DefMacro Error:Usage error.");
                return false;
            }

            string varName = param[0..param.IndexOf('=')];
            string value = param[(param.IndexOf('=') + 1)..];

            if(!MacroTool.GetStringFromString(value,interpreter,out value))
            {
                Printer.Error($"DefMacro Error:variable `{varName}` not defined");
                return false;
            }

            if(interpreter != null)
            {
                if (interpreter.LocalVariables.ContainsKey(varName))
                {
                    Printer.Error($"DefMacro Error:variable {varName} defined.");
                    return false;
                }

                interpreter.LocalVariables.Add(varName, value);
            }
            else
            {
                lock (Build.GlobalContext.GlobalContextLocker)
                {
                    if (Build.GlobalContext.GlobalEnvironmentVariables.ContainsKey(varName))
                    {
                        Printer.Error($"DefMacro Error:variable {varName} defined.");
                        return false;
                    }
                    Build.GlobalContext.GlobalEnvironmentVariables.Add(varName, value);
                }
            }

            return true;
        }

        [Api.Macro("Undef")]
        public bool UndefMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (interpreter != null)
            {
                if (interpreter.LocalVariables.ContainsKey(param))
                {
                    interpreter.LocalVariables.Remove(param);
                    return true;
                }
            }
            else
            {
                lock (Build.GlobalContext.GlobalContextLocker)
                {
                    if (Build.GlobalContext.GlobalEnvironmentVariables.ContainsKey(param))
                    {
                        Build.GlobalContext.GlobalEnvironmentVariables.Remove(param);
                        return true;
                    }
                   
                }
            }
            Printer.Error($"UndefMacro Error:can't find macro `{param}`");

            return false;
        }








    }
}
