/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Utility.cs
 * Content: Utility Source File
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
    // 临时值
    class VariableAST : IExprAST
    {
        public VariableAST(Token? token) : base(token) { }

        public Variable constVariable = new Variable();

        public override Variable Execute(Context context)
        {
            return constVariable;
        }
    }

    // 复合Expr
    class BlockAST : IExprAST
    {
        public BlockAST(Token? token) : base(token) { }

        public IExprAST[] blocks = Array.Empty<IExprAST>();

        public override Variable Execute(Context context)
        {
            Variable output = new Variable();

            foreach (var ast in blocks)
                output = ast.SafeExecute(context);

            return output;
        }
    }

    // 赋值
    class AssignmentAST : IExprAST
    {
        public string name = "";
        public IExprAST value = new NopAST(null);

        public AssignmentAST(Token? token) : base(token) { }

        public override Variable Execute(Context context)
        {
            Context.SetVariable(context, name, value.Execute(context));
            return new Variable();
        }
    }

    /// <summary>
    /// 全局变量赋值
    /// </summary>
    class GlobalAssignmentAST : IExprAST
    {
        public string name = "";
        public IExprAST value = new NopAST(null);

        public GlobalAssignmentAST(Token? token) : base(token) { }

        public override Variable Execute(Context context)
        {
            Context.SetVariable_Global(name, value.Execute(context));
            return new Variable();
        }
    }

    /// <summary>
    /// 获取变量
    /// </summary>
    class GetVariableAST : IExprAST
    {
        public string varName = "";

        public GetVariableAST(Token? token) : base(token) { }

        public override Variable Execute(Context context)
        {
            if (!Context.TryGetVariable(context, varName, out Variable? variable))
            {
                throw new RuntimeException($"Variable `{varName}` Not Found!", this);
            }
            else return variable!;
        }
    }

    /// <summary>
    /// Shell执行AST
    /// </summary>
    class ShAST : IExprAST
    {
        public ShAST(Token? token) : base(token) { }

        public IExprAST cmd = new NopAST(null);

        public override Variable Execute(Context context)
        {
            Standard.Sh sh = new Standard.Sh();

            return sh.Execute(context, new Variable[] { cmd.Execute(context) });
        }
    }
}
