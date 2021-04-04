/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    NopAST.cs
 * Content: NopAST Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;


namespace Doing.Engine.AST
{
    /// <summary>
    /// 什么都不干
    /// </summary>
    class NopAST : IExprAST
    {
        public override Variable Execute(Utility.Context context)
        {
            return new Variable();
        }

        public NopAST(Token? token) : base(token) { }
    }
}
