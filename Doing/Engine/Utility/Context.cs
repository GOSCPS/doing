/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Context.cs
 * Content: Context Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Concurrent;


namespace Doing.Engine.Utility
{
    /// <summary>
    /// 上下文
    /// </summary>
    public class Context
    {
        /// <summary>
        /// 全局变量表
        /// </summary>
        public static readonly ConcurrentDictionary<string, Variable> GlobalVariableTable
            = new ConcurrentDictionary<string, Variable>();

        /// <summary>
        /// 全局函数表
        /// </summary>
        public static readonly ConcurrentDictionary<string, Function> GlobalFunctionTable
            = new ConcurrentDictionary<string, Function>();

        /// <summary>
        /// 本地变量表
        /// </summary>
        public readonly ConcurrentDictionary<string, Variable> LocalVariableTable
            = new ConcurrentDictionary<string, Variable>();

        /// <summary>
        /// 本地对象表
        /// </summary>
        public readonly ConcurrentDictionary<string, object> LocalObjectTable
            = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 获取变量，局部变量优先
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool TryGetVariable(Context? context, string name, out Variable? variable)
        {

            if (context != null)
            {
                // 局部变量优先
                if (!context.LocalVariableTable.TryGetValue(name, out variable))
                {
                    return GlobalVariableTable.TryGetValue(name, out variable);
                }
                else return true;
            }

            // context为空
            // 从全局变量获取
            return GlobalVariableTable.TryGetValue(name, out variable);
        }

        public static void SetVariable(Context context, string name, Variable value)
        {
            context.LocalVariableTable.AddOrUpdate(name, value, (string key, Variable old) => { return value; });
        }

        public static void SetVariable_Global(string name, Variable value)
        {
            GlobalVariableTable.AddOrUpdate(name, value, (string key, Variable old) => { return value; });
        }
    }
}
