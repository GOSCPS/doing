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
    }
}
