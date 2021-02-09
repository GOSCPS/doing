/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ExpandManager.cs
 * Content: ExpandManager Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace doing.Expand
{
    /// <summary>
    /// 扩展管理器
    /// </summary>
    static class ExpandManager
    {
        /// <summary>
        /// 扩展实例列表
        /// </summary>
        private static Dictionary<Type, object> expandInstance
            = new Dictionary<Type, object>();

        /// <summary>
        /// 从文件加载扩展
        /// </summary>
        /// <param name="path">扩展路径</param>
        public static void LoadExpandFromFile(string path)
        {
            Assembly expand = Assembly.LoadFrom(path);
            Type[] allClasses = expand.GetTypes();

            foreach (var cls in allClasses)
            {
                //不重复添加
                if (expandInstance.ContainsKey(cls))
                    continue;

                //确认为扩展
                object attribute = cls.GetCustomAttribute<Api.DoingExpandAttribute>();
                if (attribute == null)
                    continue;

                //添加实例
                expandInstance.Add(cls, Activator.CreateInstance(cls));

                //检查Method
                foreach (var met in cls.GetMethods())
                {
                    attribute = met.GetCustomAttribute<Api.MacroAttribute>();

                    if (attribute != null)
                        ProcessMacro(cls, met, (Api.MacroAttribute)attribute);
                }

            }
            return;
        }


        //处理宏
        public static void ProcessMacro(
            Type cls,
            MethodInfo macroMethod,
            Api.MacroAttribute attribute)
        {
            if (expandInstance.TryGetValue(cls, out object obj))
            {

                if (macroMethod.ReturnType != typeof(bool))
                {
                    Printer.Error($"Doing Error:Expand {cls} load macro method {macroMethod.Name}\n" +
                        $"But not return bool");
                    return;
                }
                else if (macroMethod.GetParameters().Length != 2)
                {
                    Printer.Error($"Doing Error:Expand {cls} load macro method {macroMethod.Name}\n" +
                        $"But param count not 1");
                    return;
                }
                else if (macroMethod.GetParameters()[0].ParameterType != typeof(string))
                {
                    Printer.Error($"Doing Error:Expand {cls} load macro method {macroMethod.Name}\n" +
                        $"But param type not string");
                    return;
                }
                else if (macroMethod.GetParameters()[1].ParameterType != typeof(Build.Interpreter.Interpreter))
                {
                    Printer.Error($"Doing Error:Expand {cls} load macro method {macroMethod.Name}\n" +
                        $"But param type not Interpreter");
                    return;
                }

                Build.MacroManager.AddMacro(attribute.MacroName,
                    delegate (string param, Build.Interpreter.Interpreter interpreter)
                    {
                        return (bool)macroMethod.Invoke(obj, new object[] { param, interpreter });
                    });
            }
            else throw new ArgumentException($"Type {cls} in expand object dictionary was null.");
        }


    }
}
