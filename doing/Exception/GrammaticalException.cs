/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    GrammaticalException.cs
 * Content: GrammaticalException Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System.Text;

namespace doing.Exception
{
    /// <summary>
    /// 语法错误
    /// </summary>
    public class GrammaticalException : System.Exception
    {
        public Build.Interpreter.LineInfo Position { init; get; }

        public GrammaticalException(string message, Build.Interpreter.LineInfo pos)
            : base(message)
        {
            Position = pos;
        }

        public GrammaticalException(string message, Build.Interpreter.LineInfo pos,System.Exception innerexception)
            : base(message, innerexception)
        {
            Position = pos;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Error:{Position}\n");
            stringBuilder.Append(base.ToString());
            return stringBuilder.ToString();
        }
    }
}
