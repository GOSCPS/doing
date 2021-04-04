/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Logic.cs
 * Content: Logic Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;


namespace Doing.Standard
{
    class Not : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Params count isn't 1!");

            var condit = args[0];

            bool conditionResult = true;

            // 为真:
            // 非0数字
            // 值为true的bool
            // 非Empty字符串
            // 非null的object
            // 
            // 为假:
            // 数字0
            // 值为false的bool
            // 空字符串
            // null的object
            if (condit.Type == Variable.VariableType.NoType)
            {
                conditionResult = false;
            }
            else if (condit.Type == Variable.VariableType.Boolean)
            {
                conditionResult = condit.ValueBoolean;
            }
            else if (condit.Type == Variable.VariableType.Number)
            {
                if (condit.ValueNumber == 0)
                    conditionResult = false;
                else conditionResult = true;
            }
            else if (condit.Type == Variable.VariableType.String)
            {
                if (condit.ValueString == string.Empty)
                    conditionResult = false;
                else conditionResult = true;
            }
            else if (condit.Type == Variable.VariableType.Object)
            {
                if (condit.ValueObject == null)
                    conditionResult = false;
                else conditionResult = true;
            }

            return new Variable()
            {
                Type = Variable.VariableType.Boolean,
                ValueBoolean = !conditionResult
            };
        }
    }

    class And : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            Not not = new Not();

            foreach (var arg in args)
            {
                if (not.Execute(callerContext, new Variable[] { arg }).ValueBoolean != false)
                {
                    return new Variable()
                    {
                        Type = Variable.VariableType.Boolean,
                        ValueBoolean = false
                    };
                }
            }

            return new Variable()
            {
                Type = Variable.VariableType.Boolean,
                ValueBoolean = true
            };
        }
    }
}
