/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    OsDef.cs
 * Content: OsDef Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Osdef")]
    public class OsDef : Api.IMacroApi
    {

        [Api.Macro("OsDef")]
        public bool MacroMethod(string param, Build.Interpreter.Interpreter interpreter)
        {
            
            return true;
        }
    }
}
