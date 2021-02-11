/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    MacroTool.cs
 * Content: MacroTool Source Files
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
    /// 宏工具箱
    /// </summary>
    public static class MacroTool
    {

        /// <summary>
        /// 从字符串获取字符串
        /// </summary>
        /// <returns>
        /// 获取的字符串
        /// 获取失败返回null
        /// </returns>
        public static bool GetStringFromString(
            in string param, Build.Interpreter.Interpreter interpreter, out string result)
        {
            if (param == null || param.Length == 0)
            {
                result = "";
                return true;
            }

            //以$开头
            //视为变量
            if (param.StartsWith('$'))
            {
                string varName = param[1..];

                //局部变量优先
                if (interpreter != null)
                {
                    foreach (var v in interpreter.LocalVariables)
                        if (v.Key == varName)
                        {
                            result = v.Value;
                            return true;
                        }
                }
                //全局变量靠后
                lock (Build.GlobalContext.GlobalContextLocker)
                    foreach (var v in Build.GlobalContext.GlobalEnvironmentVariables)
                        if (v.Key == varName)
                        {
                            result = v.Value;
                            return true;
                        }

                //Printer.Error($"Variables {varName} not found");
                result = "";
                return false;
            }
            //\$开头
            //视为转义
            else if (param.StartsWith("\\$"))
            {
                result = param[1..];
                return true;
            }
            //以"开头
            //视为字符串
            else if (param.StartsWith('"'))
            {
                Tool.StringIterator stringIterator = new Tool.StringIterator(param);
                string strresult = stringIterator.ReadNextString();

                if(strresult == null)
                {
                    //Printer.Error($"String `{param.Replace("{","{{")}` parse error");
                    result = "";
                    return false;
                }
                else
                {
                    result = strresult;
                    return true;
                }
            }
            //以\"开头
            //视为转义
            else if (param.StartsWith("\\\""))
            {
                result = param[1..];
                return true;
            }
            //其他
            //原样输出
            else
            {
                result = param;
                return true;
            }
        }






    }
}
