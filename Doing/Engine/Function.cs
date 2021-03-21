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


namespace Doing.Engine
{

    /// <summary>
    /// 函数
    /// </summary>
    abstract class Function
    {
        public string FunctionName = "";

        /// <summary>
        /// 参数
        /// </summary>
        protected Variable[] args = Array.Empty<Variable>();

        /// <summary>
        /// 参数表达式
        /// </summary>
        public ExprAST[] argsExprs = Array.Empty<ExprAST>();

        /// <summary>
        /// 准备和执行
        /// </summary>
        public virtual Variable Start(Context context)
        {
            List<Variable> vars = new List<Variable>();

            // 设置参数
            for (int a = 0; a != argsExprs.Length; a++)
            {
                vars.Add(argsExprs[a].Execute(context));
            }
            args = vars.ToArray();

            // 执行函数
            return Execute(new Context());
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Variable Execute(Context context);
    }

    /// <summary>
    /// 自定义的Function
    /// </summary>
    class DefinedFunction : Function
    {
        /// <summary>
        /// 参数列表
        /// </summary>
        public string[] argsList = Array.Empty<string>();

        /// <summary>
        /// 函数体
        /// </summary>
        public ExprAST expr = new DoNotThingExpr();

        /// <summary>
        /// 准备和执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Variable Start(Context context)
        {
            // 检查参数长度
            if (argsExprs.Length != argsList.Length)
                throw new RuntimeException("Function call param count not same!");

            // 创建函数环境
            Context funcContext = new Context();
            for(int a=0;a != argsList.Length; a++)
            {
                // 在函数外的环境下创建函数参数
                funcContext.Variables.Add(argsList[a], argsExprs[a].Execute(context));
            }

            // 执行函数
            return Execute(funcContext);
        }

        public override Variable Execute(Context context)
        {
            return expr.Execute(context);
        }
    }
}
