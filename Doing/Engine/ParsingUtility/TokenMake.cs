/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    TokenMake.cs
 * Content: TokenMake Source File
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
    /// <summary>
    /// Token辅助操作器
    /// </summary>
    public class TokenMake
    {
        public Token[]? Tokens { get; set; }
        public long ptr = 0;

        public void Next() => ptr++;
        public void Back() => ptr--;

        public bool IsEnd() => ptr >= Tokens!.Length;

        /// <summary>
        /// 移动并判断下一个Token是否是指定类型
        /// </summary>
        /// <param name="type">要判断的类型</param>
        /// <param name="endErr">如果是End，抛出错误的信息</param>
        /// <param name="noMatchErr">如果Type不匹配，抛出错误的信息</param>
        public void NextAndCompare(TokenType type,string endErr,string noMatchErr)
        {
            Next();

            if (IsEnd())
            {
                throw new CompileException(endErr,GetLastToken());
            }

            else if (Current.type != type)
                throw new CompileException(noMatchErr, Current);

            else return;
        }

        public Token GetLastToken()
        {
            return Tokens![^1];
        }

        public Token Current
        {
            get
            {
                return Tokens![ptr];
            }
        }
    }

}