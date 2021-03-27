/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    CheckFile.cs
 * Content: CheckFile Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
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


namespace Doing.Standard
{
    /// <summary>
    /// 检查文件
    /// </summary>
    class CheckFile : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            if (File.Exists(args[0].ValueString))
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            }
            else
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
            }
        }
    }
}
