//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    RuntimeException.cs
// Content: RuntimeException Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using Doing.Engine;

namespace Doing.DException
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RuntimeException : Exception
    {
        public string Msg { get; init; }
        public new Exception? InnerException { get; init; } = null;

        public RuntimeException(string msg) : base(msg)
        {
            Msg = msg;
        }
        public RuntimeException(string msg, Exception inner) : base(msg, inner)
        {
            Msg = msg;
            InnerException = inner;
        }

        public override string ToString()
        {
            if (InnerException == null)

                if (Program.IsDebug)
                    return base.ToString();
                else
                    return "*** " + Msg;

            else
                if (Program.IsDebug)
                return base.ToString();
            else
                return "*** " + Msg + "\n" + InnerException.ToString();
        }

    }

    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RuntimePositionException : Exception
    {
        public string Msg { get; init; }

        public Engine.BuildLineInfo Position { get; init; }

        public RuntimePositionException(string msg, BuildLineInfo pos) : base(msg)
        {
            Msg = msg;
            Position = pos;
        }

        public override string ToString()
        {
            if (Program.IsDebug)
                return $"*** At {Position.Position.SourceFile.FullName} Lines {Position.LineNumber}\n" + base.ToString();
            else
                return $"*** At {Position.Position.SourceFile.FullName} Lines {Position.LineNumber}\n" + Msg;
        }
    }
}
