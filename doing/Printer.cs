/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Printer.cs
 * Content: Printer Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Text;

namespace doing
{
    /// <summary>
    /// 打印机
    /// </summary>
    static class Printer
    {
        private static readonly object locker = new object();

        public static void Error(string fmt, params object[] arg)
        {
            try { 
            lock (locker)
            {
                var backer = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Error.WriteLine(fmt, arg);

                Console.ForegroundColor = backer;
            }
            }
            catch (System.FormatException err)
            {
                Console.Error.WriteLine("Printer:String Format Error!");
                Console.Error.WriteLine($"Printer:String Base64:" +
                    $"{{{Convert.ToBase64String(Encoding.UTF8.GetBytes(fmt))}}}");
                Console.Error.WriteLine(err.ToString());
            }
        }

        public static void Warn(string fmt, params object[] arg)
        {
            try { 
            lock (locker)
            {
                var backer = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Error.WriteLine(fmt, arg);

                Console.ForegroundColor = backer;
            }
            }
            catch (System.FormatException err)
            {
                Console.Error.WriteLine("Printer:String Format Error!");
                Console.Error.WriteLine($"Printer:String Base64:" +
                    $"{{{Convert.ToBase64String(Encoding.UTF8.GetBytes(fmt))}}}");
                Console.Error.WriteLine(err.ToString());
            }
        }

        public static void Common(string fmt, params object[] arg)
        {
            try
            {
                lock (locker)
                {
                    Console.Error.WriteLine(fmt, arg);
                }
            }
            catch (System.FormatException err)
            {
                Console.Error.WriteLine("Printer:String Format Error!");
                Console.Error.WriteLine($"Printer:String Base64:" +
                    $"{{{Convert.ToBase64String(Encoding.UTF8.GetBytes(fmt))}}}");
                Console.Error.WriteLine(err.ToString());
            }
        }

        public static void Good(string fmt, params object[] arg)
        {
            try { 
            lock (locker)
            {
                var backer = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.Out.WriteLine(fmt, arg);

                Console.ForegroundColor = backer;
            }
            }
            catch (System.FormatException err)
            {
                Console.Error.WriteLine("Printer:String Format Error!");
                Console.Error.WriteLine($"Printer:String Base64:" +
                    $"{{{Convert.ToBase64String(Encoding.UTF8.GetBytes(fmt))}}}");
                Console.Error.WriteLine(err.ToString());
            }
        }
    }
}
