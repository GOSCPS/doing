/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ExpandLoader.cs
 * Content: ExpandLoader Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;


namespace Doing.Expand
{
    /// <summary>
    /// 扩展加载器
    /// </summary>
    public static class ExpandLoader
    {
        /// <summary>
        /// 扩展列表
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> expands
            = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 从文件加载扩展
        /// </summary>
        /// 
        /// <param name="file">文件</param>
        /// <param name="named">扩展Type名称</param>
        public static void LoadFromFile(string file, string named)
        {
            string index = "FILE => " + Path.GetFullPath(file.Trim()) + " => " + named.Trim();

            // 不重复添加
            if (expands.ContainsKey(index))
            {
                return;
            }

            Assembly assembly = Assembly.LoadFrom(file);
            Type? t = assembly.GetType(named);

            if (t == null)
                throw new Engine.CompileException($"Using {file} type {named} error!");

            object? obj = Activator.CreateInstance(t);

            if (obj == null)
                throw new Engine.CompileException($"Create Instance {file} type {named} error!");

            expands.TryAdd(index, obj);

            return;
        }

        /// <summary>
        /// 加载扩展
        /// </summary>
        /// 
        /// <param name="file">文件</param>
        /// <param name="named">扩展Type名称</param>
        public static void LoadFrom(string assemblyName, string named)
        {
            string index = "SYSTEM => " + assemblyName.Trim() + " => " + named.Trim();

            // 不重复添加
            if (expands.ContainsKey(index))
            {
                return;
            }

            Assembly assembly = Assembly.Load(assemblyName);
            Type? t = assembly.GetType(named);

            if (t == null)
                throw new Engine.CompileException($"UsingSystem {assemblyName} type {named} error!");

            object? obj = Activator.CreateInstance(t);

            if (obj == null)
                throw new Engine.CompileException($"Create Instance (System) {assemblyName} type {named} error!");

            expands.TryAdd(index, obj);

            return;
        }

    }



}
