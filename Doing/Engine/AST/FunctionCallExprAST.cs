/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    FunctionCallExprAST.cs
 * Content: FunctionCallExprAST Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System;
using System.Collections.Generic;


namespace Doing.Engine.AST
{
    class FunctionCallExprAST : IExprAST
    {
        public FunctionCallExprAST(Token? token) : base(token) { }

        public string funcName = "";
        public IExprAST[] args = Array.Empty<IExprAST>();

        public override Variable Execute(Context context)
        {
            if (!Context.GlobalFunctionTable.TryGetValue(funcName, out Function? func))
            {
                throw new RuntimeException($"Function `{funcName}` Not Found!", this);
            }
            else
            {
                List<Variable> variables = new List<Variable>();

                foreach (var arg in args)
                    variables.Add(arg.Execute(context));

                return func.Execute(context, variables.ToArray());
            }
        }
    }
}
