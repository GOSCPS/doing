/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Control.cs
 * Content: Control Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Security.Cryptography;
using System.Text;

namespace doing.Inner.Macro
{
    [Api.DoingExpand("doing-InnerExpand.Macro.Control", License = "GOSCPS", Version = 1)]
    public class Control
    {
        /// <summary>
        /// 结束target执行
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Break")]
        public bool BreakMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{").Replace("}", "}}");
            Printer.Common(param);
            //修改程序计数器
            //到末尾
            interpreter.ProgramCounter
                = interpreter.Source.Codes.Length - 1;
            return true;
        }

        /// <summary>
        /// 引发错误
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Error")]
        public bool ErrorMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            param = param.Replace("{", "{{").Replace("}","}}");
            Printer.Error(param);
            return false;
        }

        /// <summary>
        /// 什么都不干
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Pass")]
        public bool PassMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            return true;
        }

        /// <summary>
        /// 把当前变量名称备份然后删除
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Backup")]
        public bool BackupMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            //获取Value
            //局部变量优先
            if (interpreter != null)
            {
                foreach (var v in interpreter.LocalVariables)
                {
                    if (v.Key == param)
                    {
                        interpreter.LocalVariables.Remove(v.Key);

                        SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
                        byte[] bs = SHA512.ComputeHash(Encoding.Unicode.GetBytes(v.Key));

                        interpreter.LocalVariables.Add(Convert.ToBase64String(bs), v.Value);

                        return true;
                    }
                }
            }
            lock (Build.GlobalContext.GlobalContextLocker)
                foreach (var v in Build.GlobalContext.GlobalEnvironmentVariables)
                {
                    if (v.Key == param)
                    {
                        interpreter.LocalVariables.Remove(v.Key);

                        SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
                        byte[] bs = SHA512.ComputeHash(Encoding.Unicode.GetBytes(v.Key));

                        interpreter.LocalVariables.Add(Convert.ToBase64String(bs), v.Value);

                        return true;
                    }
                }

            Printer.Error($"BackupMacro Error:Not found variables `{param}`");

            return false;
        }

        /// <summary>
        /// 恢复备份
        /// </summary>
        /// <param name="param"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        [Api.Macro("Restore")]
        public bool RestoreMacro(string param, Build.Interpreter.Interpreter interpreter)
        {
            //Key -> SHA512 -> BASE64
            SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
            byte[] h5 = SHA512.ComputeHash(Encoding.Unicode.GetBytes(param));
            string backupName = Convert.ToBase64String(h5);

            //局部变量优先
            if (interpreter != null)
            {
                foreach (var v in interpreter.LocalVariables)
                {
                    if (v.Key == backupName)
                    {
                        interpreter.LocalVariables.Remove(backupName);

                        interpreter.LocalVariables.Add(param, v.Value);

                        return true;
                    }
                }
            }
            lock (Build.GlobalContext.GlobalContextLocker)
                foreach (var v in Build.GlobalContext.GlobalEnvironmentVariables)
                {
                    if (v.Key == backupName)
                    {
                        interpreter.LocalVariables.Remove(backupName);

                        Build.GlobalContext.GlobalEnvironmentVariables.Add(param, v.Value);

                        return true;
                    }
                }

            Printer.Error($"BackupMacro Error:Not found variables `{param}`");

            return false;
        }

    }
}
