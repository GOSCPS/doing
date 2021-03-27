/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Emiter.cs
 * Content: Emiter Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;


namespace Doing.Engine
{
    /// <summary>
    /// Awesome C#!
    /// </summary>
    static class Emiter
    {
        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="tokens"></param>
        public static void Compile(Token[] tokens)
        {
            // 忽略空输入
            if (tokens == null || tokens.Length == 0)
                return;

            // 添加标准库
            Standard.Standard.AddStandard();

            ParsingUtility.TokenMake token = new ParsingUtility.TokenMake
            {
                Tokens = tokens
            };

            while (true)
            {
                if (token.IsEnd())
                    break;

                // 解析Target
                else if (token.Current.type == TokenType.keyword_target)
                {
                    ParsingUtility.ParsingTarget.Parsing_Target(token).Execute();
                }
                // 解析函数
                else if(token.Current.type == TokenType.keyword_function)
                {
                    var func = ParsingUtility.ParsingFunction.Parsing_function(token);

                    if (!Utility.Context.GlobalFunctionTable.TryAdd(func.Name, func))
                        throw new CompileException($"Function `{func.Name}` defined!");
                }

                else throw new CompileException("Unknown Token Begin.",token.Current);
            }

        }
    }
}
