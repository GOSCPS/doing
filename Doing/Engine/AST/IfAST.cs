/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    IfAST.cs
 * Content: IfAST Source File
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
    /// <summary>
    /// if
    /// </summary>
    public class IfAST : IExprAST
    {
        public IfAST(Token? token) : base(token) { }

        public IExprAST condition = new NopAST(null);

        public IExprAST body = new NopAST(null);
        public IExprAST? elseBody;

        public override Variable Execute(Context context)
        {
            var condit = condition.Execute(context);

            bool conditionResult = true;

            // 为真:
            // 非0数字
            // 值为true的bool
            // 非Empty字符串
            // 非null的object
            // 
            // 为假:
            // 数字0
            // 值为false的bool
            // 空字符串
            // null的object
            if(condit.Type == Variable.VariableType.NoType)
            {
                conditionResult = false;
            }
            else if(condit.Type == Variable.VariableType.Boolean)
            {
                conditionResult = condit.ValueBoolean;
            }
            else if(condit.Type == Variable.VariableType.Number)
            {
                if (condit.ValueNumber == 0)
                    conditionResult = false;
                else conditionResult = true;
            }
            else if (condit.Type == Variable.VariableType.String)
            {
                if (condit.ValueString == string.Empty)
                    conditionResult = false;
                else conditionResult = true;
            }
            else if (condit.Type == Variable.VariableType.Object)
            {
                if (condit.ValueObject == null)
                    conditionResult = false;
                else conditionResult = true;
            }

            // 执行
            if (conditionResult)
                return body.Execute(context);

            else if (elseBody != null)
                return elseBody.Execute(context);

            return new Variable();
        }
    }
}
