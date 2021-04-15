/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ParsingExpr.cs
 * Content: ParsingExpr Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.AST;
using System.Collections.Generic;

namespace Doing.Engine.ParsingUtility
{
    static class ParsingExpr
    {

        // 运算符优先级
        // 从高到低
        // ()
        // * /
        // + -
        // 比较运算符(== != ...)
        // 运算符结合算法

        /// <summary>
        /// 表达式解析的开始
        /// 最低级表达式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IExprAST Begin_Expr(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Incomplete expression!");

            AST.ExprExprAST expr = new(token.Current);
            expr.LFT = High_Expr(token);

            // !
            expr.RGH = null;

            while (true)
            {
                if (token.IsEnd())
                    break;

                // 解析 + -
                if (token.Current.type == TokenType.add
                || token.Current.type == TokenType.sub)
                {
                    if (expr.RGH == null)
                    {
                        expr.op = token.Current.type;
                        token.Next();

                        // 解析右值
                        expr.RGH = High_Expr(token);
                    }
                    else
                    {
                        ExprExprAST high = new(token.Current);

                        high.op = token.Current.type;
                        token.Next();

                        high.LFT = expr;

                        high.RGH = High_Expr(token);

                        expr = high;
                    }
                }

                else break;
            }

            // 右值为空，则只返回左值
            if (expr.RGH == null)
                return expr.LFT;

            return expr;
        }

        /// <summary>
        /// 高一级表达式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IExprAST High_Expr(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Incomplete expression!");

            AST.ExprExprAST expr = new(token.Current);
            expr.LFT = Highest_Expr(token);

            // !
            expr.RGH = null;

            while (true)
            {
                if (token.IsEnd())
                    break;

                // 解析 * / 
                if (token.Current.type == TokenType.mul
                || token.Current.type == TokenType.div)
                {
                    if (expr.RGH == null)
                    {
                        expr.op = token.Current.type;
                        token.Next();

                        // 解析右值
                        expr.RGH = Highest_Expr(token);
                    }
                    else
                    {
                        ExprExprAST high = new(token.Current);

                        high.op = token.Current.type;
                        token.Next();

                        high.LFT = expr;

                        high.RGH = Highest_Expr(token);

                        expr = high;
                    }
                }

                else break;
            }

            // 右值为空，则只返回左值
            if (expr.RGH == null)
                return expr.LFT;

            return expr;
        }

        /// <summary>
        /// 最高级表达式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IExprAST Highest_Expr(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Incomplete expression!");

            // 只解析 (
            if (token.Current.type == TokenType.parentheses)
            {
                token.Next();

                IExprAST expr = Begin_Expr(token);

                // 检查)
                ParsingUtility.CheckTokenType(token, TokenType.parentheses_end);

                return expr;
            }
            // 剩下基本单位
            else if (token.Current.type == TokenType.str)
            {
                return new VariableAST(token.Current)
                {
                    constVariable = new Utility.Variable()
                    {
                        Type = Utility.Variable.VariableType.String,
                        ValueString = ParsingUtility.GetString(token)
                    }
                };
            }
            else if (token.Current.type == TokenType.number)
            {
                return new VariableAST(token.Current)
                {
                    constVariable = new Utility.Variable()
                    {
                        Type = Utility.Variable.VariableType.Number,
                        ValueNumber = ParsingUtility.GetNumber(token)
                    }
                };
            }
            else if (token.Current.type == TokenType.identifier)
            {
                string named = ParsingUtility.GetIdentifier(token);

                // 判断是否是函数调用

                // 是函数调用
                if((!token.IsEnd()) && token.Current.type == TokenType.parentheses)
                {
                    return Parsing_Expr_Function_Call_Param(token, named);
                }
                // 不是函数调用
                else
                {
                    token.Back();
                    var e = new AST.GetVariableAST(token.Current)
                    {
                        varName = named
                    };
                    token.Next();
                    return e;
                }
            }
            else throw new CompileException("Unknown token!",token.Current);
        }


        /// <summary>
        /// 表达式运算
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        private static IExprAST Parsing_Expr_Calculation(TokenMake token)
        {
            // 读取所有可以参与运算的符号
            // + - * / == !=  > >= <= < 数字 字符串 标识符 ( ) ,

            if (token.IsEnd())
                throw new CompileException("Expect Expr but get end-of-token!", token.GetLastToken());

            // 收集
            List<Token> exprToken = new();
            while (true)
            {
                if (token.IsEnd())
                    break;

                if(token.Current.type == TokenType.add
                    || token.Current.type == TokenType.sub
                    || token.Current.type == TokenType.mul
                    || token.Current.type == TokenType.div
                    || token.Current.type == TokenType.double_equal
                    || token.Current.type == TokenType.not_equal
                    || token.Current.type == TokenType.bigger
                    || token.Current.type == TokenType.bigger_equal
                     || token.Current.type == TokenType.smaller
                    || token.Current.type == TokenType.smaller_equal
                     || token.Current.type == TokenType.parentheses
                    || token.Current.type == TokenType.parentheses_end
                    || token.Current.type == TokenType.number
                    || token.Current.type == TokenType.str
                    || token.Current.type == TokenType.identifier
                    || token.Current.type == TokenType.comma){
                    exprToken.Add(token.Current);
                    token.Next();
                }
                else break;
            }
            if (exprToken.Count == 0)
                throw new CompileException("Expect Expr but get wrong token", token.Current);

            // 解析
            var exprMaker = new TokenMake() { Tokens = exprToken.ToArray() };
            IExprAST expr = Begin_Expr(exprMaker);

            if (!exprMaker.IsEnd())
                throw new CompileException("Parsing expr wrong!",exprMaker.Current);

            return expr;
        }

        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IExprAST Parsing_Expr_Function_Call_Param(TokenMake token, string funcName)
        {
            // 检查(
            ParsingUtility.CheckTokenType(token, TokenType.parentheses);

            token.Back();

            // 设置前置信息
            FunctionCallExprAST function = new(token.Current)
            {
                funcName = funcName
            };

            token.Next();

            List<IExprAST> args = new List<IExprAST>();

            // 循环获取参数
            while (true)
            {
                if (token.IsEnd())
                    throw new CompileException("Expect Function Params or `)` but get end-of-token!"
                        , token.GetLastToken());

                // )
                // 获取参数结束
                else if (token.Current.type == TokenType.parentheses_end)
                {
                    break;
                }

                // ,
                // 继续获取参数
                else if (token.Current.type == TokenType.comma)
                {
                    token.Next();
                }

                args.Add(Begin_Expr(token));
            }
            token.Next();

            function.args = args.ToArray();

            return function;
        }

        /// <summary>
        /// 检查不能参与运算的关键字（true，false，null）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static void Parsing_Expr_Check_Keyword(TokenMake token)
        {
            if (token.IsEnd())
                return;

            // 尾随 + - * / ( !
            if (token.Current.type == TokenType.add
                || token.Current.type == TokenType.sub
                || token.Current.type == TokenType.mul
                || token.Current.type == TokenType.div
                || token.Current.type == TokenType.parentheses)
            {
                Tool.Printer.WarnLine($"Warn `{token.Current.SourceFileName}` Lines {token.Current.Line}:\n" +
                    $"The keyword `true` or `false` or `null` Do not participate in calculations.\n" +
                    $"But Access Operator After them!");
            }

            return;
        }

        /// <summary>
        /// 解析一个表达式
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IExprAST Parsing_Expr(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Expect expr but get End-Of-Token!", token.GetLastToken());

            // true false null视为单独一个Expr
            if (token.Current.type == TokenType.keyword_true)
            {
                var opt = new VariableAST(token.Current)
                {
                    constVariable = new Utility.Variable()
                    {
                        Type = Utility.Variable.VariableType.Boolean,
                        ValueBoolean = true
                    }
                };
                token.Next();

                Parsing_Expr_Check_Keyword(token);
                return opt;
            }

            // false
            else if (token.Current.type == TokenType.keyword_false)
            {
                var opt = new VariableAST(token.Current)
                {
                    constVariable = new Utility.Variable()
                    {
                        Type = Utility.Variable.VariableType.Boolean,
                        ValueBoolean = false
                    }
                };
                token.Next();

                Parsing_Expr_Check_Keyword(token);
                return opt;
            }

            // null
            else if (token.Current.type == TokenType.null_token)
            {
                var opt = new VariableAST(token.Current)
                {
                    constVariable = new Utility.Variable()
                    {
                        Type = Utility.Variable.VariableType.Object,
                        ValueObject = null
                    }
                };
                token.Next();

                Parsing_Expr_Check_Keyword(token);
                return opt;
            }

            // 运算
            else
            {
                return Parsing_Expr_Calculation(token);
            }
        }
    }
}
