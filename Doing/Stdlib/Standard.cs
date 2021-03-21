/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Standard.cs
 * Content: Standard Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine;
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


namespace Doing.Stdlib
{
    /// <summary>
    /// 标准库
    /// </summary>
    class Standard
    {

        public static void AddToLibrary()
        {
            _ = Context.FunctionList.TryAdd("Print", new Printf()) ? 1 : throw new RuntimeException("Add Standard Library Error!");
        }

        class Printf : Function
        {
            public override Variable Execute(Context context)
            {
                foreach(var arg in args)
                {

                    if (arg == null)
                        continue;

                    switch(arg.type)
                    {
                        case VariableType.Number:
                            Tool.Printer.Put(arg.ValueNumber.ToString());
                            break;

                        case VariableType.String:
                            Tool.Printer.Put(arg.ValueString.ToString());
                            break;

                        case VariableType.NoType:
                            break;

                        default:
                            throw new Engine.RuntimeException("Never happen!");
                    }
                }

                return new Variable();
            }
        }









    }
}
