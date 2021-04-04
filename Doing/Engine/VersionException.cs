/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    VersionException.cs
 * Content: VersionException Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Engine
{
    /// <summary>
    /// 版本过低异常
    /// </summary>
    class VersionException : Exception
    {
        public Version RequiredVersion { get; init; }

        public VersionException(string msg, Version required) : base(msg)
        {
            RequiredVersion = required;
        }

        public override string ToString()
        {
            return $"Required Version {RequiredVersion} at least.\n" + base.ToString();
        }
    }
}
