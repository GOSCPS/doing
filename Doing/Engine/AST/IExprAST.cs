/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ExprAST.cs
 * Content: ExprAST Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Reflection.Emit;


namespace Doing.Engine.AST
{
    /// <summary>
    /// 抽象语法树
    /// </summary>
    public abstract class IExprAST
    {
        /// <summary>
        /// 源文件名称
        /// </summary>
        public string SourceFileName = "";

        /// <summary>
        /// 源文件行数
        /// </summary>
        public long SourceFileLine = 0;

        /// <summary>
        /// Aha.Awesome C#!
        /// </summary>
        /// 
        /// <param name="context">上下文信息</param>
        public abstract Utility.Variable Execute(Utility.Context context);

        /// <summary>
        /// 异常安全执行
        /// </summary>
        /// <returns></returns>
        public Utility.Variable SafeExecute(Utility.Context context)
        {
            try
            {
                return Execute(context);
            }
            catch(Exception err)
            {
                throw new RuntimeException("AST Runtime Error!", this, err);
            }
        }

        public IExprAST(Token? token)
        {
            if(token == null)
            {
                SourceFileName = "Unknown";
                SourceFileLine = -1;
                return;
            }

            SourceFileName = token.SourceFileName;
            SourceFileLine = token.Line;
        }
    }
}
