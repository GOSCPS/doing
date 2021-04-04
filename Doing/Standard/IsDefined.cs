/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    IsDefined.cs
 * Content: IsDefined Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;


namespace Doing.Standard
{
    class IsDefined : Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            if (Context.TryGetVariable(callerContext, args[0].ValueString, out _))
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            }
            else
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
            }
        }
    }

    class IsDefinedLocal : Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            if (callerContext.LocalVariableTable.ContainsKey(args[0].ValueString))
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            }
            else
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
            }
        }
    }

    class IsDefinedGlobal : Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            if (Context.GlobalVariableTable.ContainsKey(args[0].ValueString))
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = true
                };
            }
            else
            {
                return new Variable()
                {
                    Type = Variable.VariableType.Boolean,
                    ValueBoolean = false
                };
            }
        }
    }
}
