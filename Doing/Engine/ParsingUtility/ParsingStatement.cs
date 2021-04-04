/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ParsingStatement.cs
 * Content: ParsingStatement Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Generic;


namespace Doing.Engine.ParsingUtility
{
    static class ParsingStatement
    {
        // 解析赋值语句
        public static AST.IExprAST Parsing_Assignment_Statement(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Expect Assignment Statement but get End-Of-Token!"
                    , token.GetLastToken());

            // 以标识符开头
            // 局部变量
            if (token.Current.type == TokenType.identifier)
            {
                // 获取变量名
                string named = token.Current.ValueString;
                token.Next();

                // 确保 =
                if (token.IsEnd())
                    throw new CompileException("Expect `=` but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.equal)
                    throw new CompileException($"Expect `=` but get `{token.Current.type}`!", token.GetLastToken());
                token.Next();

                // 获取表达式
                AST.AssignmentAST assignment = new AST.AssignmentAST(token.Current)
                {
                    name = named,
                    value = ParsingExpr.Parsing_Expr(token)
                };

                // 确保 ;
                if (token.IsEnd())
                    throw new CompileException("Expect `;` but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.semicolon)
                    throw new CompileException($"Expect `;` but get `{token.Current.type}`!", token.GetLastToken());
                token.Next();

                return assignment;
            }

            // 以global开头
            // 全局变量
            else if (token.Current.type == TokenType.keyword_global)
            {
                token.Next();

                // 确保 标识符(变量名)
                if (token.IsEnd())
                    throw new CompileException("Expect identifier but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.identifier)
                    throw new CompileException($"Expect identifier but get `{token.Current.type}`!", token.GetLastToken());

                // 获取变量名
                string named = token.Current.ValueString;
                token.Next();


                // 确保 =
                if (token.IsEnd())
                    throw new CompileException("Expect `=` but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.equal)
                    throw new CompileException($"Expect `=` but get `{token.Current.type}`!", token.Current);
                token.Next();

                // 获取表达式
                AST.GlobalAssignmentAST assignment = new AST.GlobalAssignmentAST(token.Current)
                {
                    name = named,
                    value = ParsingExpr.Parsing_Expr(token)
                };


                // 确保 ;
                if (token.IsEnd())
                    throw new CompileException("Expect `;` but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.semicolon)
                    throw new CompileException($"Expect `;` but get `{token.Current.type}`!", token.Current);
                token.Next();


                return assignment;
            }

            else throw new CompileException("Unknown Assignment Statement Begin!", token.Current);
        }

        /// <summary>
        /// 解析if
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static AST.IExprAST Parsing_Statement_If(TokenMake token)
        {
            AST.IfAST @if = new AST.IfAST(token.Current);

            token.Next();
            @if.condition = ParsingExpr.Parsing_Expr(token);

            @if.body = Parsing_Statement(token);

            if ((!token.IsEnd()) && token.Current.type == TokenType.keyword_else)
            {
                token.Next();
                @if.elseBody = Parsing_Statement(token);
            }

            return @if;
        }

        public static AST.IExprAST Parsing_Statement_Sh(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Expect `sh` but get End-Of-Token!", token.GetLastToken());

            AST.ShAST sh = new AST.ShAST(token.Current);
            token.Next();

            // 解析表达式
            sh.cmd = ParsingExpr.Parsing_Expr(token);

            // 确保结尾;
            if (token.IsEnd())
                throw new CompileException("Expect `;` but get End-Of-Token!", token.GetLastToken());

            else if (token.Current.type != TokenType.semicolon)
                throw new CompileException($"Expect `;` but get `{token.Current.type:G}`!", token.Current);

            token.Next();

            return sh;
        }

        public static AST.IExprAST Parsing_Statement(TokenMake token)
        {
            if (token.IsEnd())
                throw new CompileException("Expect statement but get End-Of-Token!", token.GetLastToken());

            // 空语句
            if (token.Current.type == TokenType.semicolon)
            {
                var opt = new AST.NopAST(token.Current);
                token.Next();
                return opt;
            }

            // 语句块
            else if (token.Current.type == TokenType.curlyBraces)
            {
                List<AST.IExprAST> exprs = new List<AST.IExprAST>();
                AST.BlockAST block = new AST.BlockAST(token.Current);
                token.Next();

                while (true)
                {
                    if (token.IsEnd())
                        throw new CompileException("Open curly braces never close!", token.GetLastToken());

                    // }则结束
                    else if (token.Current.type == TokenType.curlyBraces_end)
                    {
                        token.Next();
                        break;
                    }

                    // 解析语句块内语句
                    else exprs.Add(Parsing_Statement(token));
                }

                block.blocks = exprs.ToArray();
                return block;
            }

            // global赋值语句
            else if (token.Current.type == TokenType.keyword_global)
            {
                return Parsing_Assignment_Statement(token);
            }

            // 标识符
            else if (token.Current.type == TokenType.identifier)
            {
                token.Next();

                // Next是=
                // 视为本地赋值
                if ((!token.IsEnd()) && token.Current.type == TokenType.equal)
                {
                    token.Back();
                    return Parsing_Assignment_Statement(token);
                }

                // 视作Expr
                else
                {
                    token.Back();

                    var expr = ParsingExpr.Parsing_Expr(token);

                    // 确保 ;存在
                    if (token.IsEnd())
                        throw new CompileException("Expect `;` but get End-Of-Token!", token.GetLastToken());
                    if (token.Current.type != TokenType.semicolon)
                        throw new CompileException($"Expect `;` but get `{token.Current.type}`!", token.GetLastToken());

                    return expr;
                }
            }
            // if
            else if (token.Current.type == TokenType.keyword_if)
            {
                return Parsing_Statement_If(token);
            }
            // sh
            else if (token.Current.type == TokenType.keyword_sh)
            {
                return Parsing_Statement_Sh(token);
            }
            // 表达式
            else
            {
                var expr = ParsingExpr.Parsing_Expr(token);

                // 确保 ;存在
                if (token.IsEnd())
                    throw new CompileException("Expect `;` but get End-Of-Token!", token.GetLastToken());
                if (token.Current.type != TokenType.semicolon)
                    throw new CompileException($"Expect `;` but get `{token.Current.type}`!", token.GetLastToken());

                return expr;
            }
        }
    }
}
