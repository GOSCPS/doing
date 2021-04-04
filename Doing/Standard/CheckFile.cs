/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    CheckFile.cs
 * Content: CheckFile Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System.IO;


namespace Doing.Standard
{
    /// <summary>
    /// 检查文件
    /// </summary>
    class CheckFile : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            string f = args[0].ValueString;

            if (File.Exists(f))
            {
                Tool.Printer.PutLine($"Check File `{f}` ... \033[32mOK\033[0m");
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            }
            else
            {
                Tool.Printer.PutLine($"Check File `{f}` ... \033[31mErr\033[0m");
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
            }
        }
    }
}
