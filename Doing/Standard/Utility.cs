/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Utility.cs
 * Content: Utility Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System;


namespace Doing.Standard
{
    class GetEnvironmentVariable : Engine.Utility.Function
    {

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            else if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            string? env = Environment.GetEnvironmentVariable(args[0].ValueString);

            if (env == null)
                throw new Engine.RuntimeException($"Environment Variable `{args[0].ValueString}` Not Found!");

            return new Variable()
            {
                Type = Variable.VariableType.String,
                ValueString = env
            };
        }
    }

    class GetDoingVersion : Engine.Utility.Function
    {

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 0)
                throw new Engine.RuntimeException("Needn't param!");

            return new Variable()
            {
                Type = Variable.VariableType.String,
                ValueString = Program.DoingVersion.ToString()
            };
        }

    }
}
