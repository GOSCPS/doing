/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Printer.cs
 * Content: Printer Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Tool
{
    /// <summary>
    /// 打印
    /// </summary>
    public static class Printer
    {
        public static readonly object locker = new();

        public static void PutLine(string fmt, params object?[] args)
        {
            lock (locker)
            {
                Console.Out.WriteLine(fmt, args);
            }
        }

        public static void WarnLine(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Out.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void ErrLine(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void OkLine(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Error.WriteLine(fmt, args);
                Console.ForegroundColor = colored;
            }
        }


        public static void Put(string fmt, params object?[] args)
        {
            lock (locker)
            {
                Console.Out.Write(fmt, args);
            }
        }

        public static void Warn(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Out.Write(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void Err(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        public static void Ok(string fmt, params object?[] args)
        {
            lock (locker)
            {
                var colored = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Error.Write(fmt, args);
                Console.ForegroundColor = colored;
            }
        }

        /// <summary>
        /// 仅在Debug模式下打印
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        public static void Debug(string fmt, params object?[] args)
        {
            if (Program.IsDebug)
                lock (locker)
                {
                    var colored = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Error.Write(fmt, args);
                    Console.ForegroundColor = colored;
                }
        }

        /// <summary>
        /// 仅在Debug模式下打印
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        public static void DebugLine(string fmt, params object?[] args)
        {
            if (Program.IsDebug)
                lock (locker)
                {
                    var colored = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Error.WriteLine(fmt, args);
                    Console.ForegroundColor = colored;
                }
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatErr(string str)
        {
            Err(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatErrLine(string str)
        {
            ErrLine(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatPut(string str)
        {
            Put(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatPutLine(string str)
        {
            PutLine(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatWarn(string str)
        {
            Warn(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatWarnLine(string str)
        {
            WarnLine(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatOk(string str)
        {
            Ok(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatOkLine(string str)
        {
            OkLine(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatDebug(string str)
        {
            Debug(str.Replace("{", "{{").Replace("}", "}}"));
        }

        /// <summary>
        /// 不格式化的输出
        /// </summary>
        /// <param name="str"></param>
        public static void NoFormatDebugLine(string str)
        {
            DebugLine(str.Replace("{", "{{").Replace("}", "}}"));
        }
    }
}
