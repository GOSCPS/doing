using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Exception
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RuntimeException : System.Exception
    {
        public string ErrMsg { get; init; }
        public System.Exception? InnerEx { get; init; } = null;

        public Engine.BuildFileInfo? ErrFile { get; init; } = null;
        public Engine.BuildLineInfo? ErrLine { get; init; } = null;

        public RuntimeException(string msg) : base(msg) 
        {
            ErrMsg = msg;
        }

        public RuntimeException(string msg, Engine.BuildFileInfo file, Engine.BuildLineInfo line) : base(msg)
        {
            ErrMsg = msg;
            ErrFile = file;
            ErrLine = line;
        }

        public RuntimeException(string msg,System.Exception inner) : base(msg, inner)
        {
            ErrMsg = msg;
            InnerEx = inner;
        }

        public override string ToString()
        {
            if (Program.IsDebug)
            {
                if (ErrFile != null && ErrLine != null)
                    return $"At `{ErrFile.FileName}` Lines {ErrLine.LineNumber}\n" + base.ToString();
                else
                    return base.ToString();
            }
            else
            {
                if (InnerEx != null)
                {
                    if (ErrFile != null && ErrLine != null)
                        return ErrMsg + $"\n\tAt `{ErrFile.FileName}` Lines {ErrLine.LineNumber}\n" + InnerEx.ToString();

                    else return ErrMsg + "\n" + InnerEx.ToString();
                }
                else
                {
                    if (ErrFile != null && ErrLine != null)
                        return ErrMsg + $"\n\tAt `{ErrFile.FileName}` Lines {ErrLine.LineNumber}";

                    else return ErrMsg;
                }
            }
        }
    }
}
