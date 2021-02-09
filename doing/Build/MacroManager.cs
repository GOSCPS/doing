/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    MacroManager.cs
 * Content: MacroManager Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System.Collections.Generic;

namespace doing.Build
{
    /// <summary>
    /// 宏管理器
    /// </summary>
    public static class MacroManager
    {
        /// <summary>
        /// 全局锁
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// 宏列表
        /// </summary>
        private static Dictionary<string, Api.IMacroApi.Macro>
            MacroList = new Dictionary<string, Api.IMacroApi.Macro>();

        /// <summary>
        /// 添加宏
        /// </summary>
        /// <param name="callName">调用名称</param>
        /// <param name="macro">宏的委托</param>
        /// <returns>
        /// 添加成功返回true，否则返回false。调用名称重复可能导致宏添加失败。委托为null可能导致添加失败。
        /// </returns>
        public static bool AddMacro(string callName, Api.IMacroApi.Macro macro)
        {
            lock (locker)
            {
                if (macro == null)
                    return false;

                if (MacroList.ContainsKey(callName))
                {
                    return false;
                }
                else
                {
                    MacroList.Add(callName, macro);
                }
                return true;
            }
        }

        /// <summary>
        /// 删除宏
        /// </summary>
        /// <param name="callName">宏的调用名称</param>
        public static void RemoveMacro(string callName)
        {
            lock (locker)
                MacroList.Remove(callName);
            return;
        }

        /// <summary>
        /// 获取宏
        /// </summary>
        /// <param name="callName">宏的调用名称</param>
        /// <returns>找到的宏。找不到返回null。</returns>
        public static bool GetMacro(string callName, out Api.IMacroApi.Macro macro)
        {
            lock (locker)
            {
                if (MacroList.TryGetValue(callName, out macro))
                {
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 调用宏
        /// </summary>
        /// <param name="callName">调用名称</param>
        /// <param name="param">调用参数</param>
        /// <param name="interpreter">调用的解释器</param>
        /// <returns>
        /// 调用成功返回宏的返回值，
        /// 调用失败返回null。
        /// 找不到宏导致调用失败。
        /// </returns>
        public static bool? CallMacro(string callName, string param, Interpreter.Interpreter interpreter)
        {
            lock (locker)
            {
                if (MacroList.TryGetValue(callName, out Api.IMacroApi.Macro macro))
                {
                    return macro.Invoke(param, interpreter);
                }
                else
                {
                    Printer.Error($"Macro `{callName}` not found");
                    return null;
                }
            }
        }


    }
}
