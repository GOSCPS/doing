/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    DefineFunction.cs
 * Content: DefineFunction Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Engine.Utility
{
    /// <summary>
    /// 用户定义的函数 
    /// </summary>
    public class DefineFunction : Function
    {
        public string name = "";
        public override string Name { get { return name; } }

        /// <summary>
        /// 参数名列表
        /// </summary>
        public string[] argsName = Array.Empty<string>();

        /// <summary>
        /// 参数体
        /// </summary>
        public AST.IExprAST funcBody = new AST.NopAST(null);

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != argsName.Length)
                throw new CompileException($"Expect params count {argsName.Length} but get {args.Length}!");

            // 创建函数运行时
            Context context = new Context();
            for (int a = 0; a < args.Length; a++)
            {
                if (!context.LocalVariableTable.TryAdd(argsName[a], args[a]))
                    throw new CompileException($"Add param `{argsName[a]}` error!");
            }

            return funcBody.SafeExecute(context);
        }
    }
}
