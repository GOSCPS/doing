/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    OsDef.cs
 * Content: OsDef Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.OsDef", License = "GOSCPS", Version = 1)]
    public class OsDef : Api.IMacroApi
    {

        /// <summary>
        /// OsDef
        /// Usage:OsName:Key=Value
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("OsDef")]
        public bool MacroMethod(string param, Build.Interpreter.Interpreter interpreter)
        {
            //判断操作系统
            if (!(param.Contains('=')
                && param.Contains(':')))
            {
                Printer.Error("OsDef usage error.right is `OsName:Key=Value`");
                return false;
            }

            string osName = param[0..param.IndexOf(':')];

            if (osName != Environment.OSVersion.Platform.ToString())
                return true;

            //Parse Key=Value

            string key_value_pair = param[param.IndexOf(':')..];

            if (!key_value_pair.Contains('='))
            {
                Printer.Error("OsDef usage error.right is `OsName:Key=Value`");
                return false;
            }
            else
            {
                string var_name = key_value_pair[0..key_value_pair.IndexOf('=')];
                string var_value = key_value_pair[(1 + key_value_pair.IndexOf('='))..];
                //全局变量
                if (interpreter == null)
                {
                    lock (Build.GlobalContext.GlobalContextLocker)
                        Build.GlobalContext.GlobalEnvironmentVariables.Add(var_name, var_value);
                }
                //局部变量
                else
                {
                    interpreter.LocalVariables.Add(var_name, var_value);
                }
            }

            return true;
        }

        /// <summary>
        /// 全局版本OsDef
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("GlobalOsDef")]
        public bool MacroMethod_Global(string param, Build.Interpreter.Interpreter interpreter)
        {
            return MacroMethod(param, null);
        }
    }
}
