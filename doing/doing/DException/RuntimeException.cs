//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    RuntimeException.cs
// Content: RuntimeException Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doing.Engine;

namespace Doing.DException
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RuntimeException : Exception
    {
        public string Msg { get; init; }

        public RuntimeException(string msg) : base(msg)
        {
            this.Msg = msg;
        }

        public override string ToString()
        {
            if (Program.IsDebug)
                return base.ToString();
            else return "*** " + Msg;
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
            if(Program.IsDebug)
                return $"*** At {Position.Position.SourceFile.FullName} Lines {Position.LineNumber}\n" + base.ToString();
            else
                return $"*** At {Position.Position.SourceFile.FullName} Lines {Position.LineNumber}\n" + Msg;
        }
    }
}
