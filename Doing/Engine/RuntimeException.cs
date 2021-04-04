/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    RuntimeException.cs
 * Content: RuntimeException Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Engine
{
    /// <summary>
    /// 运行时异常
    /// </summary>
    public class RuntimeException : Exception
    {
        // 是否打印堆栈
        public static bool PrintStack { get; set; } = false;

        public new AST.IExprAST? Source { get; init; }
        public string? ErrorMsg { get; init; }

        /// <summary>
        /// 运行时错误
        /// </summary>
        /// <param name="msg">错误信息</param>
        /// <param name="source">错误AST</param>
        public RuntimeException(string msg, AST.IExprAST source) :
            base(msg)
        {
            this.Source = source;
            ErrorMsg = msg;
        }

        public RuntimeException(string msg, AST.IExprAST source, Exception inner) :
            base(msg, inner)
        {
            this.Source = source;
            ErrorMsg = msg;
        }

        public RuntimeException(string msg, Exception inner) :
            base(msg, inner)
        {
            ErrorMsg = msg;
            Source = null;
        }

        public RuntimeException(string msg) :
            base(msg)
        {
            ErrorMsg = msg;
            Source = null;
        }

        public override string ToString()
        {
            if (PrintStack)
            {
                if (Source != null)
                    return $"Doing Runtime Exception!\n" +
                        $"\tError AST:{Source.GetType().Name}\n" +
                        $"\tError File `{Source.SourceFileName}` Lines `{Source.SourceFileLine}`\n" +
                        base.ToString();
                else
                    return $"Doing Runtime Exception!\n" + base.ToString();
            }
            else
            {
                if (Source != null)
                {
                    string output = $"Doing Runtime Exception!\n" +
                        $"\tError AST:{Source.GetType().Name}\n" +
                        $"\tError File `{Source.SourceFileName}` Lines `{Source.SourceFileLine}`\n" +
                        $"\t{ErrorMsg}";

                    // 打印Inner异常
                    if (InnerException != null)
                        output += $"\n{InnerException}";

                    return output;
                }
                else
                {
                    string output = $"Doing Runtime Exception!\n" +
                        $"\t{ErrorMsg}";

                    // 打印Inner异常
                    if (InnerException != null)
                        output += $"\n{InnerException}";

                    return output;
                }
            }
        }
    }
}
