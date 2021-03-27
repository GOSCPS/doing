/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Target.cs
 * Content: Target Source File
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


namespace Doing.Engine.Utility
{
    /// <summary>
    /// Target
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Target主体
        /// </summary>
        public AST.IExprAST body = new AST.NopAST(null);

        /// <summary>
        /// Target名称
        /// </summary>
        public string name = "";

        /// <summary>
        /// Target依赖
        /// </summary>
        public string[] deps = Array.Empty<string>();

        /// <summary>
        /// 执行Target
        /// </summary>
        public void Execute()
        {
            body.SafeExecute(new Context());
        }
    }
}
