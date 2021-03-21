/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Parsing.cs
 * Content: Parsing Source File
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
    /// 抽象表达式语法树接口
    /// 语法解析器的基础
    /// </summary>
    abstract class ExprAST
    {
        /// <summary>
        /// 行号
        /// </summary>
        public long Line = 0;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName = "Unknown";

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Variable Execute(Context context);
    }

    /// <summary>
    /// 变量类型
    /// </summary>
    enum VariableType
    {
        NoType,
        String,
        Number
    }

    /// <summary>
    /// 变量
    /// </summary>
    class Variable
    {
        public string VariableName = "";

        public VariableType type = VariableType.NoType;


        private string str = "";
        private long num = 0;

        public string ValueString
        {
            get
            {
                if (type != VariableType.String)
                    throw new TypeException($"Access string but variable type is {type}");

                return str;
            }
            set
            {
                if (type != VariableType.String)
                    throw new TypeException($"Access string but variable type is {type}");

                str = value;
            }
        }
        public long ValueNumber
        {
            get
            {
                if (type != VariableType.Number)
                    throw new TypeException($"Access number but variable type is {type}");

                return num;
            }
            set
            {
                if (type != VariableType.Number)
                    throw new TypeException($"Access number but variable type is {type}");

                num = value;
            }
        }

    }

    /// <summary>
    /// 语法解析器
    /// </summary>
    static class Parsing
    {
        class TokenMake
        {
            public Token[]? Tokens { get; set; }
            public long ptr = 0;

            public void Next() => ptr++;
            public void Back() => ptr--;

            public bool IsEnd() => ptr >= Tokens!.Length;
            public void IsEnd(string err) { 
                if(ptr >= Tokens!.Length)
                {
                    throw new RuntimeException(err);
                }
                else
                {
                    return;
                }
            }
            public void IsEnd(string err,Token token)
            {
                if (ptr >= Tokens!.Length)
                {
                    throw new RuntimeException(err, token);
                }
                else
                {
                    return;
                }
            }

            public Token Current
            {
                get
                {
                    return Tokens![ptr];
                }
            }
        }

        /// <summary>
        /// 解析一条语句
        /// </summary>
        /// <returns></returns>
        private static ExprAST ParsingStatement(ref TokenMake token)
        {
            token.IsEnd("Unexpected Statement EOF!");

            // 空语句
            if(token.Current.type == TokenType.semicolon)
            {
                token.Next();
                return new DoNotThingExpr();
            }
            // 复合语句
            else if(token.Current.type == TokenType.curlyBraces)
            {
                ExprBlock exprBlock = new ExprBlock();
                List<ExprAST> exprs = new List<ExprAST>();

                token.Next();

                while (true)
                {
                    token.IsEnd("Unexpected EOF! Miss `}`!");

                    if (token.Current.type == TokenType.curlyBraces_end)
                        break;

                    else
                        exprs.Add(ParsingStatement(ref token));
                }

                token.Next();
                exprBlock.exprs = exprs.ToArray();
                return exprBlock;
            }
            //以Ident开头
            else if(token.Current.type == TokenType.identifier)
            {
                string ident = token.Current.ValueString;
                token.Next();

                // 赋值
                if (token.Current.type == TokenType.equal)
                {
                    token.Next();

                    AssignmentExpr expr = new AssignmentExpr
                    {
                        IsGlobal = false,
                        VarName = ident,

                        value = ParsingExpr(ref token)
                    };

                    // 检查;
                    token.IsEnd("Unexpected EOF! Miss token `;`!");
                    if (token.Current.type != TokenType.semicolon)
                    {
                        throw new RuntimeException("Miss token `;`!");
                    }
                    token.Next();

                    return expr;
                }

                // 函数调用
                else if (token.Current.type == TokenType.parentheses)
                {
                    token.Next();
                    List<ExprAST> argsExprs = new List<ExprAST>();

                    // 解析参数列表
                    while (true)
                    {
                        token.IsEnd("Unexpected Expr EOF!Miss token `)`!");

                        if (token.Current.type == TokenType.parentheses_end)
                            break;

                        else
                        {
                            argsExprs.Add(ParsingExpr(ref token));

                            token.IsEnd("Unexpected Expr EOF!Miss token `)`!");

                            if (token.Current.type == TokenType.parentheses_end)
                                break;

                            else if (token.Current.type != TokenType.comma)
                                throw new RuntimeException("Expect token `,`!");
                            else
                                token.Next();
                        }
                    }
                    token.Next();

                    CallExpr callExpr = new CallExpr
                    {
                        args = argsExprs.ToArray()
                    };

                    // 搜索函数
                    if (Context.FunctionList.TryGetValue(ident, out Function? func))
                    {
                        callExpr.function = func;
                    }
                    else
                    {
                        throw new RuntimeException($"Unknown Function Name:{ident}!");
                    }

                    // 检查;
                    token.IsEnd("Unexpected EOF! Miss token `;`!");
                    if (token.Current.type != TokenType.semicolon)
                    {
                        throw new RuntimeException("Miss token `;`!");
                    }
                    token.Next();

                    return callExpr;
                }
                else
                {
                    throw new RuntimeException("Unknown token usage!",token.Current);
                }
            }
            // global全局变量赋值
            else if(token.Current.type == TokenType.keyword_global)
            {
                token.Next();

                // 检查变量名
                token.IsEnd("Unexpected EOF! Miss identifier!");

                if(token.Current.type != TokenType.identifier)
                {
                    throw new RuntimeException("Miss identifier!");
                }
                string varName = token.Current.ValueString;
                token.Next();

                // 检查=
                token.IsEnd("Unexpected EOF! Miss Token `=`!");

                if (token.Current.type != TokenType.equal)
                {
                    throw new RuntimeException("Miss Token `=`!");
                }
                token.Next();

                // 赋值
                AssignmentExpr expr = new AssignmentExpr
                {
                    IsGlobal = true,
                    VarName = varName
                };
                expr.value = ParsingExpr(ref token);

                // 检查;
                token.IsEnd("Unexpected EOF! Miss token `;`!");
                if (token.Current.type != TokenType.semicolon)
                {
                    throw new RuntimeException("Miss token `;`!");
                }
                token.Next();

                return expr;
            }
            // If语句
            else if(token.Current.type == TokenType.keyword_if)
            {
                return ParsingIf(ref token);
            }
            // return语句
            else if(token.Current.type == TokenType.keyword_return)
            {
                token.Next();

                var expr = ParsingExpr(ref token);

                // 检查;
                token.IsEnd("Miss Token `;`!");
                if (token.Current.type != TokenType.semicolon)
                    throw new RuntimeException("Miss Token `;`!",token.Current);

                token.Next();

                return expr;
            }
            else
            {
                throw new RuntimeException("Unknown token for statement.",token.Current);
            }
        }


        /// <summary>
        /// 解析If
        /// </summary>
        /// <returns></returns>
        private static ExprAST ParsingIf(ref TokenMake token)
        {
            IfExpr ifExpr = new IfExpr();

            token.IsEnd("Unexpected If Statement EOF!");
            if (token.Current.type != TokenType.keyword_if)
                throw new RuntimeException("Current Token isn't `if`!", token.Current);

            token.Next();

            // 获取条件 
            ifExpr.condition = ParsingExpr(ref token);

            // 获取语句
            ifExpr.ifIsTrue = ParsingStatement(ref token);

            // 有else
            if ((!token.IsEnd()) && token.Current.type == TokenType.keyword_else)
            {
                token.Next();
                ifExpr.ifIsFalse = ParsingStatement(ref token);
            }

            return ifExpr;
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static ExprAST ParsingExpr(ref TokenMake token)
        {
            token.IsEnd("Unexpected Expr EOF!");

            // 以标识符开头
            if(token.Current.type == TokenType.identifier)
            {
                Token ll = token.Current;

                token.Next();

                // 末尾
                // 视作变量
                if (token.IsEnd())
                    return new GetVariableExpr() { VarName = ll.ValueString };

                // 四则运算
                else if (token.Current.type == TokenType.add ||
                    token.Current.type == TokenType.sub ||
                    token.Current.type == TokenType.mul ||
                    token.Current.type == TokenType.div)
                {
                    CalculationExpr calculationExpr = new CalculationExpr();

                    calculationExpr.left = new GetVariableExpr() { VarName = ll.ValueString };
                    calculationExpr.sign = token.Current.type;

                    // 解析右值
                    token.Next();
                    calculationExpr.right = ParsingExpr(ref token);
                    
                    return calculationExpr;
                }

                // 函数调用
                else if (token.Current.type == TokenType.parentheses)
                {
                    token.Next();
                    List<ExprAST> argsExprs = new List<ExprAST>();

                    // 解析参数列表
                    while (true)
                    {
                        token.IsEnd("Unexpected Expr EOF!Miss token `)`!");

                        if (token.Current.type == TokenType.parentheses_end)
                            break;

                        else
                        {
                            argsExprs.Add(ParsingExpr(ref token));

                            token.IsEnd("Unexpected Expr EOF!Miss token `)`!");

                            if (token.Current.type != TokenType.comma)
                                throw new RuntimeException("Expect token `,`!");
                            else
                                token.Next();
                        }
                    }
                    token.Next();

                    CallExpr callExpr = new CallExpr();
                    callExpr.args = argsExprs.ToArray();

                    // 搜索函数
                    if (Context.FunctionList.TryGetValue(ll.ValueString, out Function? func))
                    {
                        callExpr.function = func;
                    }
                    else
                    {
                        throw new RuntimeException($"Unknown Function Name:{ll.ValueString}!");
                    }

                    return callExpr;
                }
                else
                {
                    // 未知：视作变量
                    return new GetVariableExpr() { VarName = ll.ValueString };
                }

            }
            // 数字
            else if(token.Current.type == TokenType.number)
            {
                long num = token.Current.ValueNumber;
                token.Next();

                // 末尾
                // 返回数字
                if (token.IsEnd())
                {
                    TermExpr term = new TermExpr();
                    term.variable = new Variable()
                    {
                        type = VariableType.Number,
                        ValueNumber = num
                    };
                    return term;
                }

                // 运算
                else if (token.Current.type == TokenType.add ||
                    token.Current.type == TokenType.sub ||
                    token.Current.type == TokenType.mul ||
                    token.Current.type == TokenType.div)
                {
                    CalculationExpr calculationExpr = new CalculationExpr();

                    calculationExpr.left = new TermExpr() { 
                        variable  = new Variable()
                        {
                            type = VariableType.Number,
                            ValueNumber = num
                        }
                    };

                    calculationExpr.sign = token.Current.type;

                    // 解析右值
                    token.Next();
                    calculationExpr.right = ParsingExpr(ref token);

                    return calculationExpr;
                }
                else
                {
                    TermExpr term = new TermExpr();
                    term.variable = new Variable()
                    {
                        type = VariableType.Number,
                        ValueNumber = num
                    };
                    return term;
                }
            }
            // 字符串
            else if(token.Current.type == TokenType.str)
            {
                string str = token.Current.ValueString;
                token.Next();

                // 末尾
                // 返回字符串
                if (token.IsEnd())
                {
                    TermExpr term = new TermExpr();
                    term.variable = new Variable()
                    {
                        type = VariableType.String,
                        ValueString = str
                    };
                    return term;
                }

                // 追加字符串
                else if (token.Current.type == TokenType.add)
                {
                    CalculationExpr calculationExpr = new CalculationExpr();

                    calculationExpr.left = new TermExpr()
                    {
                        variable = new Variable()
                        {
                            type = VariableType.String,
                            ValueString = str
                        }
                    };

                    calculationExpr.sign = token.Current.type;

                    // 解析右值
                    token.Next();
                    calculationExpr.right = ParsingExpr(ref token);

                    return calculationExpr;
                }
                else
                {
                    TermExpr term = new TermExpr();
                    term.variable = new Variable()
                    {
                        type = VariableType.String,
                        ValueString = str
                    };
                    return term;
                }
            }
            // 小括号
            else if(token.Current.type == TokenType.parentheses)
            {
                token.Next();
                var exp = ParsingExpr(ref token);

                token.IsEnd("Unexpected Expr EOF!Miss token `)`!");

                if (token.Current.type != TokenType.parentheses_end)
                    throw new RuntimeException("Miss token `)`!",token.Current);

                token.Next();

                // 四则运算
                if ((!token.IsEnd())
                    &&
                    (token.Current.type == TokenType.add ||
                    token.Current.type == TokenType.sub ||
                    token.Current.type == TokenType.mul ||
                    token.Current.type == TokenType.div))
                {
                    CalculationExpr calculationExpr = new CalculationExpr
                    {
                        left = exp,

                        sign = token.Current.type
                    };

                    // 解析右值
                    token.Next();
                    calculationExpr.right = ParsingExpr(ref token);

                    return calculationExpr;
                }
                else
                    return exp;
            }
            else
            {
                throw new RuntimeException("Unknown Expr Token!",token.Current);
            }
        }

        /// <summary>
        /// 解析Target
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static Target ParsingTarget(ref TokenMake token)
        {
            if (token.Current.type != TokenType.keyword_target)
                throw new RuntimeException("Parsing target but first token isn't target!", token.Current);

            token.Next();
            token.IsEnd("Expect target identifier!");

            // 获取target名称
            if (token.Current.type != TokenType.identifier)
                throw new RuntimeException("Expect Identifier!", token.Current);

            Target target = new Target();
            target.TargetName = token.Current.ValueString;

            // 获取依赖
            token.Next();
            token.IsEnd("Expect target body!");

            // 有: 为依赖
            if (token.Current.type == TokenType.colon)
            {
                List<string> deps = new List<string>();

                token.Next();
                token.IsEnd("Expect target depend!");

                while((!token.IsEnd()) && token.Current.type == TokenType.identifier)
                {
                    deps.Add(token.Current.ValueString);
                    token.Next();
                }

                target.Deps = deps.ToArray();
            }

            // 获取语句
            target.Body = ParsingStatement(ref token);

            // DEBUG
            /*Tool.Printer.Put(target.TargetName);
            foreach (var a in target.Deps)
            {
                Tool.Printer.Put($"\tDeps:{a}");
            }*/

            return target;
        }

        /// <summary>
        /// 解析函数
        /// </summary>
        private static void ParsingFunction(ref TokenMake token)
        {
            token.IsEnd("Unexpected Function EOF!");

            if (token.Current.type != TokenType.keyword_function)
                throw new RuntimeException("Current token isn't `function`!");

            token.Next();

            // 读取标识符
            token.IsEnd("Expect Function Identifier!");
            if (token.Current.type != TokenType.identifier)
                throw new RuntimeException("Expect token `identifier`!");

            string funcName = token.Current.ValueString;

            // 读取参数列表
            token.Next();
            token.IsEnd("Expect Function Identifier!");
            if (token.Current.type != TokenType.parentheses)
                throw new RuntimeException("Expect token `(`!");

            token.Next();


            List<string> args = new List<string>();
            while (true)
            {
                token.IsEnd("Unexpected EOF! Expect token `)`!");

                if (token.Current.type == TokenType.parentheses_end)
                    break;

                // 检查 参数列表
                else if(token.Current.type == TokenType.identifier)
                {
                    args.Add(token.Current.ValueString);

                    token.Next();
                    token.IsEnd("Unexpected EOF! Expect token `)`!");

                    if (token.Current.type == TokenType.comma)
                    {
                        token.Next();
                        continue;
                    }
                    else if (token.Current.type == TokenType.parentheses_end)
                        break;
                    else throw new RuntimeException("Unexpected function param list!");
                }
                else throw new RuntimeException("Unexpected function param list!");
            }
            token.Next();

            // 获取函数体
            ExprAST expr = ParsingStatement(ref token);

            DefinedFunction function = new DefinedFunction
            {
                argsList = args.ToArray(),
                expr = expr,
                FunctionName = funcName
            };

            // 注册
            if(!Context.FunctionList.TryAdd(funcName, function))
            {
                Tool.Printer.Err($"Function `{funcName}` was defined!");
            }
            return;
        }

        /// <summary>
        /// 解析语句
        /// </summary>
        /// <param name="tokens"></param>
        public static void ParsingToken(Token[] tokens)
        {
            TokenMake token = new TokenMake()
            {
                Tokens = tokens
            };

            /* *** 语句 *** */
            // 赋值语句:Ident = Expr;
            // 函数调用语句:Func(Arg1,Arg2...)
            // 复合语句:{Expr;Expr;Expr;...}
            // 流程控制语句:if(Expr) 语句
            // 空语句;

            while (!token.IsEnd())
            {
                // 空语句
                if(token.Current.type == TokenType.semicolon)
                {
                    token.Next();
                }
                // target
                else if(token.Current.type == TokenType.keyword_target)
                {
                    var t = ParsingTarget(ref token);
                    t.Body.Execute( t.context);
                }
                // function
                else if(token.Current.type == TokenType.keyword_function)
                {
                    ParsingFunction(ref token);
                }
                else
                {
                    throw new RuntimeException("Unknown Token!Isn't `target`!");
                }
            }
        }
    }
}
