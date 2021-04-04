/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ParsingFunction.cs
 * Content: ParsingFunction Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Generic;


namespace Doing.Engine.ParsingUtility
{
    /// <summary>
    /// 解析函数
    /// </summary>
    public static class ParsingFunction
    {
        /// <summary>
        /// 解析函数
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Utility.Function Parsing_function(TokenMake token)
        {
            // 解析Function名称
            token.NextAndCompare(TokenType.identifier,
                "Expect function name. But get End-Of-Token!",
                "Expect function name.");

            string funcName = token.Current.ValueString;

            // 解析(
            token.NextAndCompare(TokenType.parentheses,
                "Expect `(` . But get End-Of-Token!",
                "Expect `(` .");

            // 解析参数
            token.Next();
            List<string> args = new List<string>();

            while (true)
            {
                if (token.IsEnd())
                    throw new CompileException("Open `)` never close.");

                else if (token.Current.type == TokenType.parentheses_end)
                {
                    token.Next();
                    break;
                }

                else if (token.Current.type == TokenType.identifier)
                {
                    args.Add(token.Current.ValueString);
                    token.Next();
                }

                else throw new CompileException("Unknown Token for function params!", token.Current);
            }

            Utility.DefineFunction define = new Utility.DefineFunction
            {
                funcBody = ParsingStatement.Parsing_Statement(token),
                name = funcName,
                argsName = args.ToArray()
            };

            return define;
        }
    }
}
