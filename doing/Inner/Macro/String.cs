/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    String.cs
 * Content: String Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.String", License = "GOSCPS", Version = 1)]
    public class String
    {


        [Api.Macro("StringAppend")]
        public bool StringAppendMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains("+="))
            {
                Printer.Error("StringAppendMacro Error:Usage error.");
                return false;
            }

            string varName = param[0..param.IndexOf("+=")].Trim();
            string adder = param[(param.IndexOf("+=") + 2)..].Trim();
            
            //送进GetString
            if(!MacroTool.GetStringFromString(adder,interpreter,out adder))
            {
                Printer.Error($"StringAppendMacro Error:Parse value error.");
                return false;
            }


                if (interpreter != null && interpreter.LocalVariables.ContainsKey(varName))
                {
                    interpreter.LocalVariables[varName] = interpreter.LocalVariables[varName] + adder;
                    return true;
                }
                else
                {
                    lock (Build.GlobalContext.GlobalContextLocker)
                    {
                        if (Build.GlobalContext.GlobalEnvironmentVariables.ContainsKey(varName))
                        {
                            Build.GlobalContext.GlobalEnvironmentVariables[varName] =
                                Build.GlobalContext.GlobalEnvironmentVariables[varName] + adder;
                            return true;
                        }
                    }
                }

            Printer.Error($"StringAppendMacro Error:variables `{varName}` not found.");
            return false;
        }

        [Api.Macro("StringAppendStr")]
        public bool StringAppendStrMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains("+="))
            {
                Printer.Error("StringAppendStrMacro Error:Usage error.");
                return false;
            }

            string varName = param[0..param.IndexOf("+=")].Trim();
            string adder = param[(param.IndexOf("+=") + 2)..].Trim();

            Tool.StringIterator iterator = new Tool.StringIterator(adder);
            adder = iterator.ReadNextString();

            if(adder == null)
            {
                Printer.Error("StringAppendStrMacro Error:Read format string error.");
                return false;
            }

            if (interpreter != null && interpreter.LocalVariables.ContainsKey(varName))
            {
                interpreter.LocalVariables[varName] = interpreter.LocalVariables[varName] + adder;
                return true;
            }
            else
            {
                lock (Build.GlobalContext.GlobalContextLocker)
                {
                    if (Build.GlobalContext.GlobalEnvironmentVariables.ContainsKey(varName))
                    {
                        Build.GlobalContext.GlobalEnvironmentVariables[varName] =
                            Build.GlobalContext.GlobalEnvironmentVariables[varName] + adder;
                        return true;
                    }
                }
            }

            Printer.Error($"StringAppendStrMacro Error:variables `{varName}` not found.");
            return false;
        }

    }
}
