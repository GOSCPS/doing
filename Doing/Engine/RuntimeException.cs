/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    RuntimeException.cs
 * Content: RuntimeException Source File
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
    /// 运行时异常
    /// </summary>
    public class RuntimeException : Exception
    {
        public new AST.IExprAST Source { get; init; }
        public string? ErrorMsg { get; init; }

        /// <summary>
        /// 运行时错误
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="source">错误AST</param>
        public RuntimeException(string msg,AST.IExprAST source) :
            base (msg)
        {
            this.Source = source;
            ErrorMsg = msg;
        }

        public RuntimeException(string msg, AST.IExprAST source,Exception inner) :
            base(msg, inner)
        {
            this.Source = source;
            ErrorMsg = msg;
        }

        public RuntimeException(string msg,Exception inner) :
            base(msg, inner)
        {
            ErrorMsg = msg;
            Source = new AST.NopAST(null);
        }

        public RuntimeException(string msg) :
            base(msg)
        {
            ErrorMsg = msg;
            Source = new AST.NopAST(null);
        }

        public override string ToString()
        {
            return $"Doing Runtime Exception!\n" +
                $"{ErrorMsg}" +
                $"Error AST:{Source.GetType().Name}\n" +
                $"Error File `{Source.SourceFileName}` Lines `{Source.SourceFileLine}`\n" +
                base.ToString();
        }
    }
}
