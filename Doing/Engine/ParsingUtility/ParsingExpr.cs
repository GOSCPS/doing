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
        /// <summary>
        /// 表达式运算
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        private static IExprAST Parsing_Expr_Calculation(TokenMake token)
        {
            // 末尾
            if (token.IsEnd())
                throw new CompileException("Expect part of expr!", token.GetLastToken());

            // 不接受true false null
            if (token.Current.type == TokenType.keyword_true
                || token.Current.type == TokenType.null_token
                || token.Current.type == TokenType.keyword_false)
                throw new CompileException($"Token `{token.Current.type:G} couldn't be a part of expr`");

            // 运算左值
            AST.IExprAST? lft;

            // 以标识符开头
            if (token.Current.type == TokenType.identifier)
            {
                string idetn = token.Current.ValueString;
                token.Next();

                // 末尾，返回值
                if (token.IsEnd())
                    return new GetVariableAST(token.GetLastToken()) { varName = idetn };

                // ( 视为函数调用
                else if (token.Current.type == TokenType.parentheses)
                {
                    return Parsing_Expr_Function_Call_Param(token, idetn);
                }

                // +-*/ 视为运算 设置左值
                else if (token.Current.type == TokenType.add
                    || token.Current.type == TokenType.sub
                    || token.Current.type == TokenType.mul
                    || token.Current.type == TokenType.div)
                {
                    lft = new GetVariableAST(token.GetLastToken()) { varName = idetn };
                }

                // 其他 返回
                else return new GetVariableAST(token.GetLastToken()) { varName = idetn };
            }

            // 以数字开头
            else if (token.Current.type == TokenType.number)
            {
                long num = token.Current.ValueNumber;
                token.Next();

                // 末尾，返回值
                if (token.IsEnd())
                    return new VariableAST(token.GetLastToken())
                    {
                        constVariable = new Utility.Variable
                        {
                            Type = Utility.Variable.VariableType.Number,
                            ValueNumber = num
                        }
                    };

                // +-*/ 视为运算 设置左值
                else if (token.Current.type == TokenType.add
                    || token.Current.type == TokenType.sub
                    || token.Current.type == TokenType.mul
                    || token.Current.type == TokenType.div)
                {
                    lft = new VariableAST(token.GetLastToken())
                    {
                        constVariable = new Utility.Variable
                        {
                            Type = Utility.Variable.VariableType.Number,
                            ValueNumber = num
                        }
                    };
                }

                // 其他 返回
                else return new VariableAST(token.GetLastToken())
                {
                    constVariable = new Utility.Variable
                    {
                        Type = Utility.Variable.VariableType.Number,
                        ValueNumber = num
                    }
                };
            }

            // 以字符串开头
            else if (token.Current.type == TokenType.str)
            {
                string str = token.Current.ValueString;
                token.Next();

                // 末尾，返回值
                if (token.IsEnd())
                    return new VariableAST(token.GetLastToken())
                    {
                        constVariable = new Utility.Variable
                        {
                            Type = Utility.Variable.VariableType.String,
                            ValueString = str
                        }
                    };

                // +-*/ 视为运算 设置左值
                else if (token.Current.type == TokenType.add
                    || token.Current.type == TokenType.sub
                    || token.Current.type == TokenType.mul
                    || token.Current.type == TokenType.div)
                {
                    lft = new VariableAST(token.GetLastToken())
                    {
                        constVariable = new Utility.Variable
                        {
                            Type = Utility.Variable.VariableType.String,
                            ValueString = str
                        }
                    };
                }

                // 其他 返回
                else return new VariableAST(token.GetLastToken())
                {
                    constVariable = new Utility.Variable
                    {
                        Type = Utility.Variable.VariableType.String,
                        ValueString = str
                    }
                };
            }

            else throw new CompileException("Unknwon Expr Begin!", token.Current);

            AST.ExprExprAST expr = new ExprExprAST(token.Current)
            {
                LFT = lft,
                op = token.Current.type
            };

            // 移动到下一个Expr
            token.Next();

            expr.RGH = Parsing_Expr_Calculation(token);

            return expr;
        }

        /// <summary>
        /// 函数调用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IExprAST Parsing_Expr_Function_Call_Param(TokenMake token, string funcName)
        {
            if (token.IsEnd())
                throw new CompileException("Expect Function Params but get end-of-token!", token.GetLastToken());

            if (token.Current.type != TokenType.parentheses)
                throw new CompileException($"Expect `(` but get {token.Current.type:G}!", token.GetLastToken());
            token.Next();

            // 设置前置信息
            FunctionCallExprAST function = new FunctionCallExprAST(token.Current)
            {
                funcName = funcName
            };

            List<IExprAST> args = new List<IExprAST>();

            // 循环获取参数
            while (true)
            {
                if (token.IsEnd())
                    throw new CompileException("Expect Function Params or `)` but get end-of-token!"
                        , token.GetLastToken());

                // )
                else if (token.Current.type == TokenType.parentheses_end)
                {
                    break;
                }

                // ,
                else if (token.Current.type == TokenType.comma)
                {
                    token.Next();
                }

                args.Add(Parsing_Expr(token));
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
