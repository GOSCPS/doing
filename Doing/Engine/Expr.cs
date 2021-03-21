/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Expr.cs
 * Content: Expr Source File
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
    /// 什么都不干的Expr
    /// </summary>
    class DoNotThingExpr : ExprAST
    {
        public override Variable Execute(Context context)
        {
            return new Variable();
        }
    }

    /// <summary>
    /// 四则表达式
    /// </summary>
    class CalculationExpr : ExprAST
    {
        public TokenType sign;

        public ExprAST left = new DoNotThingExpr();
        public ExprAST right = new DoNotThingExpr();

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public override Variable Execute(Context context)
        {
            if(left == null || right == null)
                throw new RuntimeException("Left or right is null!");

            // 运算
            Variable output = new Variable();

            Variable lft = left.Execute(context);
            Variable rgh = right.Execute(context);

            // 要求类型相同
            if(lft.type != rgh.type)
            {
                throw new RuntimeException("Incorrect operation type!");
            }

            // 字符串仅接受+
            if(lft.type == VariableType.String)
            {
                if(sign != TokenType.add)
                {
                    throw new RuntimeException("Type String only addition can be performed!");
                }

                output.type = VariableType.String;
                output.ValueString = lft.ValueString + rgh.ValueString;
            }
            // 数学运算
            else if (lft.type == VariableType.Number)
            {
                output.type = VariableType.Number;

                output.ValueNumber = sign switch
                {
                    TokenType.add => lft.ValueNumber + rgh.ValueNumber,
                    TokenType.sub => lft.ValueNumber - rgh.ValueNumber,
                    TokenType.mul => lft.ValueNumber * rgh.ValueNumber,
                    TokenType.div => lft.ValueNumber / rgh.ValueNumber,
                    _ => throw new RuntimeException("Operation not supported by number type!"),
                };
            }
            else
            {
                throw new RuntimeException("Type NoType cannot perform calculation!");
            }

            return output;
        }
    }

    /// <summary>
    /// 变量表达式
    /// </summary>
    class TermExpr : ExprAST
    {
        public Variable variable = new Variable();

        public override Variable Execute(Context context)
        {
            return variable;
        }
    }

    /// <summary>
    /// 函数调用表达式
    /// </summary>
    class CallExpr : ExprAST
    {
        /// <summary>
        /// 参数
        /// </summary>
        public ExprAST[] args = Array.Empty<ExprAST>();

        /// <summary>
        /// 函数
        /// </summary>
        public Function? function = null;

        public override Variable Execute(Context context)
        {
            if(function == null)
            {
                throw new RuntimeException("Call Function or exprs of function is null");
            }

            function.argsExprs = args;

            // 调用函数
            // 携带本地堆栈
            return function.Start(context);
        }
    }

    /// <summary>
    /// 表达式块
    /// </summary>
    class ExprBlock : ExprAST
    {
        public ExprAST[] exprs = Array.Empty<ExprAST>();

        /// <summary>
        /// 返回最后一个表达式的结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Variable Execute(Context context)
        {
            if (exprs == null)
                throw new RuntimeException("ExprBlock's exprs is null");

            Variable variable = new Variable();

            foreach (var expr in exprs)
                variable = expr.Execute(context);

            return variable;
        }
    }

    /// <summary>
    /// 判断表达式
    /// </summary>
    class IfExpr : ExprAST
    {
        public ExprAST ifIsTrue = new DoNotThingExpr();
        public ExprAST? ifIsFalse = null;

        public ExprAST condition = new DoNotThingExpr();

        public override Variable Execute(Context context)
        {
            if (condition == null || ifIsTrue == null)
                throw new RuntimeException("IfExpr's condition or trueBlock is null");

            var result = condition.Execute(context);

            switch (result.type)
            {
                // NoType
                // 0
                // ""
                // 为false
                // 否则都为true
                case VariableType.NoType:
                     if (ifIsFalse == null)
                         return new Variable();
                     else return ifIsFalse.Execute(context);


                case VariableType.Number:
                    if(result.ValueNumber == 0)
                    {
                        if (ifIsFalse == null)
                            return new Variable();
                        else return ifIsFalse.Execute( context);
                    }
                    else
                    {
                        return ifIsTrue.Execute( context);
                    }

                case VariableType.String:
                    if (result.ValueString == "")
                    {
                        if (ifIsFalse == null)
                            return new Variable();
                        else return ifIsFalse.Execute( context);
                    }
                    else
                    {
                        return ifIsTrue.Execute( context);
                    }

                default:
                    throw new RuntimeException("Never happen!");
            }
        }
    }

    /// <summary>
    /// 返回表达式
    /// </summary>
    class ReturnExpr : ExprAST
    {
        public ExprAST? ret = null;

        public override Variable Execute(Context context)
        {
            if (ret == null)
                throw new RuntimeException("ReturnExpr ret is null");

            return ret.Execute(context);
        }
    }

    /// <summary>
    /// 赋值
    /// </summary>
    class AssignmentExpr : ExprAST
    {
        /// <summary>
        /// 是否为全局变量
        /// </summary>
        public bool IsGlobal = false;

        public string VarName = "";
        public ExprAST value = new DoNotThingExpr();

        public override Variable Execute(Context context)
        {
            Variable varibale = value.Execute(context);

            if (IsGlobal)
            {
                Context.SetVariable_Global(VarName, varibale);
            }
            else
            {
                Context.SetVariable(VarName, varibale, context);
            }
            return new Variable();
        }
    }

    /// <summary>
    /// 获取变量表达式
    /// </summary>
    class GetVariableExpr : ExprAST
    {
        public string VarName = "";

        public override Variable Execute(Context context)
        {
            if(Context.TryGetVariable(VarName,out Variable? vars, context))
            {
                return vars!;
            }
            else
            {
                throw new RuntimeException($"Unknown Variable Name:{VarName}!");
            }
        }
    }
}
