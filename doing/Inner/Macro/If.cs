/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    If.cs
 * Content: If Source Files
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
    /// 判断
    /// </summary>
    [Api.DoingExpand("doing-InnerExpand.Macro.If", License = "GOSCPS", Version = 1)]
    public class If
    {
        [Api.Macro("Ifdef")]
        public bool IfdefMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            bool found = MacroTool.GetStringFromString($"${param}", interpreter, out string _);

            if(interpreter == null)
            {
                Printer.Error("IfdefMacro Error:Ifdef can only use in target.");
                return false;
            }

            //找到变量
            //找到endif设置pass
            if (found)
            {
                int pc = interpreter.ProgramCounter+1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                }
            }
            //未找到
            //endif前设为pass
            else
            {
                int pc = interpreter.ProgramCounter+1;

                for(;pc < interpreter.Source.Codes.Length; pc++)
                {
                    if(interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                    interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                }
            }

            Printer.Error("IfdefMacro Error:Not Found `endif`");
            return false;
        }


        [Api.Macro("Ifndef")]
        public bool IfndefMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            bool found = MacroTool.GetStringFromString($"${param}", interpreter, out string _);

            if (interpreter == null)
            {
                Printer.Error("IfndefMacro Error:Ifdef can only use in target.");
                return false;
            }

            //找到变量
            //找到endif设置pass
            if (!found)
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                }
            }
            //未找到
            //endif前设为pass
            else
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                    interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                }
            }

            Printer.Error("IfndefMacro Error:Not Found `endif`");
            return false;
        }


        [Api.Macro("Ifeq")]
        public bool IfeqMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains("=="))
            {
                Printer.Error("IfeqMacro Error:Usage error");
                return false;
            }

            string leftVar = param[0..param.IndexOf("==")].Trim();
            string rightVar = param[(param.IndexOf("==")+2)..].Trim();

            if(!(MacroTool.GetStringFromString(leftVar,interpreter,out leftVar)
                && MacroTool.GetStringFromString(rightVar, interpreter, out rightVar)))
            {
                Printer.Error($"IfeqMacro Error:variables {leftVar} or {rightVar} not defined");
                return false;
            }

            //执行代码
            if(leftVar.Trim() == rightVar.Trim())
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                }
            }
            //忽略代码
            else
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                    interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                }
            }

            Printer.Error("IfeqMacro Error:Not Found `endif`");
            return false;
        }

        [Api.Macro("Ifneq")]
        public bool IfneqMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains("!="))
            {
                Printer.Error("IfneqMacro Error:Usage error");
                return false;
            }

            string leftVar = param[0..param.IndexOf("!=")].Trim();
            string rightVar = param[(param.IndexOf("!=") + 2)..].Trim();

            if (!(MacroTool.GetStringFromString(leftVar, interpreter, out leftVar)
                && MacroTool.GetStringFromString(rightVar, interpreter, out rightVar)))
            {
                Printer.Error($"IfeqMacro Error:variables {leftVar} or {rightVar} not defined");
                return false;
            }

            //执行代码
            if (leftVar.Trim() != rightVar.Trim())
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                }
            }
            //忽略代码
            else
            {
                int pc = interpreter.ProgramCounter + 1;

                for (; pc < interpreter.Source.Codes.Length; pc++)
                {
                    if (interpreter.Source.Codes[pc].Item1 == "endif")
                    {
                        interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                        return true;
                    }
                    interpreter.Source.Codes[pc].Item1 = "\\\"Pass\"{\"\"}";
                }
            }

            Printer.Error("IfneqMacro Error:Not Found `endif`");
            return false;
        }


    }
}
