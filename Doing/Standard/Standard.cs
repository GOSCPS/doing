/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Standard.cs
 * Content: Standard Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace Doing.Standard
{
    /// <summary>
    /// 标准库
    /// </summary>
    public static class Standard
    {
        /// <summary>
        /// 添加标准库
        /// </summary>
        public static void AddStandard()
        {
            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(Print).Name, new Print()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(Print).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsUnix).Name, new IsUnix()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsUnix).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsWin).Name, new IsWin()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsWin).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsMac).Name, new IsMac()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsMac).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsLinux).Name, new IsLinux()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsLinux).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsDefined).Name, new IsDefined()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsDefined).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsDefinedLocal).Name, new IsDefinedLocal()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsDefinedLocal).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(IsDefinedGlobal).Name, new IsDefinedGlobal()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(IsDefinedGlobal).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(Sh).Name, new Sh()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(Sh).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(And).Name, new And()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(And).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(Not).Name, new Not()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(Not).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(Export).Name, new Export()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(Export).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(Format).Name, new Format()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(Format).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(ExecuteScript).Name, new ExecuteScript()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(ExecuteScript).Name}` But it Defined!");

            if (!Engine.Utility.Context.GlobalFunctionTable.TryAdd(typeof(GetEnvironmentVariable).Name, new GetEnvironmentVariable()))
                throw new Engine.RuntimeException($"Add Standard Library Function `{typeof(GetEnvironmentVariable).Name}` But it Defined!");
        }



    }
}
