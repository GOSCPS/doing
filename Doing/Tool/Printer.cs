/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Printer.cs
 * Content: Printer Source File
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
using System.Timers;
using System.Xml;
using System.Xml.Linq;


namespace Doing.Tool
{
    /// <summary>
    /// 打印
    /// </summary>
    public static class Printer
    {
        private static readonly object locker = new object();

        public static void Put(string fmt,params object?[] args)
        {
            lock (locker)
            {
                Console.Out.WriteLine(fmt,args);
            }
        }

        public static void Warn(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Out.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void Err(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void Ok(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Error.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }
    }
}
