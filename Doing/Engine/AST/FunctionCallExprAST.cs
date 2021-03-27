/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    FunctionCallExprAST.cs
 * Content: FunctionCallExprAST Source File
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


namespace Doing.Engine.AST
{
    class FunctionCallExprAST : IExprAST
    {
        public FunctionCallExprAST(Token? token) : base(token) { }

        public string funcName = "";
        public IExprAST[] args = Array.Empty<IExprAST>();

        public override Variable Execute(Context context)
        {
            if(!Context.GlobalFunctionTable.TryGetValue(funcName,out Function? func))
            {
                throw new RuntimeException($"Function `{funcName}` Not Found!",this);
            }
            else
            {
                List<Variable> variables = new List<Variable>();

                foreach (var arg in args)
                    variables.Add(arg.Execute(context));

                return func.Execute(context,variables.ToArray());
            }
        }
    }
}
