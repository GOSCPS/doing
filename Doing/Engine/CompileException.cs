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
    class CompileException : Exception
    {
        public readonly Token? token = null;

        private string msg = "";

        public CompileException(string msg) : base(msg)
        {
            this.msg = msg;
        }

        public CompileException(string msg, Token token) : base(msg)
        {
            this.token = token;
            this.msg = msg;
        }

        public override string ToString()
        {
            if (Program.IsDebug)
            {
                if (token != null)
                    return $"At File `{token.SourceFileName}` Lines {token.Line} because of {token.type}\n" + base.ToString();
                else
                    return base.ToString();
            }
            else
            {
                if (token != null)
                    return $"At File `{token.SourceFileName}` Lines {token.Line} because of {token.type}\n" + msg;
                else
                    return msg;
            }
        }
    }
}
