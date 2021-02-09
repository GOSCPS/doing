/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Def.cs
 * Content: Def Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Def", License = "GOSCPS", Version = 1)]
    public class Def
    {
        /// <summary>
        /// Key = Value
        /// </summary>
        /// <returns></returns>
        [Api.Macro("Def")]
        public bool MacroDef(string param, Build.Interpreter.Interpreter interpreter)
        {
            if (!param.Contains('='))
            {
                Printer.Error("OsDef usage error.right is `OsName:Key=Value`");
                return false;
            }
            else
            {
                string var_name = param[0..param.IndexOf('=')];
                string var_value = param[(1 + param.IndexOf('='))..];
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
        /// Key = Value
        /// 全局
        /// </summary>
        /// <returns></returns>
        [Api.Macro("GlobalDef")]
        public bool GlobalMacroDef(string param, Build.Interpreter.Interpreter interpreter)
        {
            return MacroDef(param, null);
        }

    }
}
