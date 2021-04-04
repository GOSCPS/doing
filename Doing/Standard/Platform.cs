/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Platform.cs
 * Content: Platform Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System.Runtime.InteropServices;


namespace Doing.Standard
{
    class IsUnix : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 0)
                throw new Engine.RuntimeException("Needn't param!");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            else
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
        }
    }

    class IsWin : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 0)
                throw new Engine.RuntimeException("Needn't param!");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            else
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
        }
    }

    class IsMac : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 0)
                throw new Engine.RuntimeException("Needn't param!");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            else
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
        }
    }

    class IsLinux : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 0)
                throw new Engine.RuntimeException("Needn't param!");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            else
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
        }
    }
}
