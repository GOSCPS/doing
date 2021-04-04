/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Print.cs
 * Content: Print Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System;
using System.Text;


namespace Doing.Standard
{
    /// <summary>
    /// 打印
    /// </summary>
    class Print : Engine.Utility.Function
    {
        public override string Name { get { return "Print"; } }

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var arg in args)
            {
                if (arg == null)
                    continue;

                switch (arg.Type)
                {
                    case Variable.VariableType.Boolean:
                        builder.Append(arg.ValueBoolean);
                        break;

                    case Variable.VariableType.Number:
                        builder.Append(arg.ValueNumber);
                        break;

                    case Variable.VariableType.Object:
                        builder.Append(arg.ValueObject);
                        break;

                    case Variable.VariableType.String:
                        builder.Append(arg.ValueString);
                        break;

                    default:
                        break;
                }
            }

            lock (Tool.Printer.locker)
                Console.Write(builder.ToString());

            return new Variable();
        }
    }
}
