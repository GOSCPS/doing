/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    DoingExpandAttribute.cs
 * Content: DoingExpandAttribute Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace doing.Api
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DoingExpandAttribute : Attribute
    {
        public string Name { get; set; }
        public string License { get; set; }
        public int Version { get; set; }

        public DoingExpandAttribute(string name)
        {
            Name = name;
        }
    }
}
