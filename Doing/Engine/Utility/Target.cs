/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Target.cs
 * Content: Target Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Engine.Utility
{
    /// <summary>
    /// Target
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Target主体
        /// </summary>
        public AST.IExprAST body = new AST.NopAST(null);

        /// <summary>
        /// Target名称
        /// </summary>
        public string name = "";

        /// <summary>
        /// Target依赖
        /// </summary>
        public string[] deps = Array.Empty<string>();

        /// <summary>
        /// 执行Target
        /// </summary>
        public void Execute()
        {
            body.SafeExecute(new Context());
        }
    }
}
