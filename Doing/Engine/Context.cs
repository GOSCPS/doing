/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Context.cs
 * Content: Context Source File
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
    /// 上下文
    /// </summary>
    class Context
    {
        /// <summary>
        /// 全局上下文
        /// </summary>
        public static Context GlobalContext = new Context();

        /// <summary>
        /// (全局)函数列表
        /// </summary>
        public static ConcurrentDictionary<string, Function> FunctionList
             = new ConcurrentDictionary<string, Function>();

        /// <summary>
        /// 锁
        /// </summary>
        public object locker = new object();

        /// <summary>
        /// 变量列表
        /// </summary>
        public Dictionary<string, Variable> Variables
            = new Dictionary<string, Variable>();

        /// <summary>
        /// 当前函数名称
        /// </summary>
        public string CurrentFunctionName = "";

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetVariable(string name, out Variable? variable, Context context)
        {
            lock (context.locker)
            {
                // 从本地变量中寻找
                if (context.Variables.TryGetValue(name, out variable))
                    return true;
                else
                    // 从全局变量寻找
                    return GlobalContext.Variables.TryGetValue(name, out variable);
            }
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        /// <param name="context"></param>
        public static void SetVariable(string name, Variable variable, Context context)
        {
            lock (context.locker)
            {
                if (context.Variables.ContainsKey(name))
                    context.Variables.Remove(name);

                context.Variables.Add(name, variable);
            }
        }

        /// <summary>
        /// 全局设置变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        public static void SetVariable_Global(string name, Variable variable)
        {
            if (GlobalContext.Variables.ContainsKey(name))
                GlobalContext.Variables.Remove(name);

            GlobalContext.Variables.Add(name, variable);

        }
    }
}
