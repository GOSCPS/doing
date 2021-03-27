/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Function.cs
 * Content: Function Source File
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
    /// 函数
    /// </summary>
    public abstract class Function
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public virtual string Name { get { return ""; } }

        /// <summary>
        /// 调用函数
        /// </summary>
        /// 
        /// <param name="callerContext">调用者上下文</param>
        /// <param name="args">调用参数</param>
        /// 
        /// <returns>函数返回值</returns>
        public abstract Variable Execute(Context callerContext,Variable[] args);
    }
}
