/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ParsingTarget.cs
 * Content: ParsingTarget Source File
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


namespace Doing.Engine.ParsingUtility
{
    class ParsingTarget
    {

        /// <summary>
        /// 解析Target
        /// 输入Token时的位置:keyword_target
        /// </summary>
        public static Utility.Target Parsing_Target(TokenMake token)
        {
            Utility.Target target = new Utility.Target();

            // 解析Target Ident
            token.NextAndCompare(TokenType.identifier,
                "Expect target name. But get End-Of-Token!",
                "Expect target name.");

            target.name = token.Current.ValueString;

            // 获取依赖列表
            token.Next();
            
            if(token.IsEnd())
                throw new CompileException("Expect deps or statement. But get End-Of-Token!", token.GetLastToken());


            // 如果有: 则之后所有identifier识别为依赖
            List<string> deps = new List<string>();

            if (token.Current.type == TokenType.colon)
            {
                token.Next();

                while (true)
                {
                    if (token.IsEnd())
                        throw new CompileException("Expect deps or statement. But get End-Of-Token!", token.GetLastToken());

                    if (token.Current.type == TokenType.identifier)
                        deps.Add(token.Current.ValueString);

                    // 检测到非identifier(语句)则退出
                    else break;

                    token.Next();
                }
            }
            target.deps = deps.ToArray();

            // 解析语句
            target.body = ParsingStatement.Parsing_Statement(token);


            return target;
        }

    }
}
