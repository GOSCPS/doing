/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Format.cs
 * Content: Format Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System.Collections.Generic;
using System.Text;


namespace Doing.Standard
{
    /// <summary>
    /// 字符串格式化
    /// </summary>
    public class Format : Engine.Utility.Function
    {

        public override Variable Execute(Context callerContext, Variable[] args)
        {
            // 第一个参数为格式化字符串
            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param format type isn't string!");

            string commandS = args[0].ValueString;
            StringBuilder command;
            bool replaced = true;

            // 替换变量
            for (int count = 0; count < 1024 && replaced; count++)
            {
                replaced = false;
                command = new StringBuilder();

                for (int ptr = 0; ptr < commandS.Length; ptr++)
                {

                    // ${}视为变量
                    if (commandS[ptr] == '$')
                    {
                        // $$ 视为$
                        ptr++;

                        if (ptr >= commandS.Length)
                            throw new Engine.RuntimeException("Miss token `$` or `{`!");

                        if (commandS[ptr] == '$')
                            command.Append('$');

                        // 非$ 视为变量
                        // 获取变量名
                        else
                        {
                            // 有变量
                            replaced = true;

                            // 检查{
                            if (ptr >= commandS.Length)
                                throw new Engine.RuntimeException("Miss token `{`!");

                            if (commandS[ptr] != '{')
                                throw new Engine.RuntimeException("Expect token `{`!");

                            ptr++;

                            // 获取变量名
                            StringBuilder varName = new StringBuilder();

                            while (true)
                            {
                                if (ptr >= commandS.Length)
                                    throw new Engine.RuntimeException("Miss token `}`!");

                                else if (commandS[ptr] == '}')
                                    break;

                                else varName.Append(commandS[ptr]);

                                ptr++;
                            }

                            // 读取变量
                            if (!Context.TryGetVariable(callerContext, varName.ToString(), out Variable? variable))
                            {
                                throw new Engine.RuntimeException($"Variable `{varName}` Not Found!");
                            }
                            else
                            {
                                // 仅支持string
                                if (variable!.Type != Variable.VariableType.String)
                                    throw new Engine.RuntimeException($"Variable `{varName}`'s Type isn't String!");

                                command.Append(variable.ValueString);
                            }
                        }
                    }
                    else
                    {
                        command.Append(commandS[ptr]);
                    }
                }

                commandS = command.ToString();
            }

            // 添加其他
            List<object?> fmtArgs = new List<object?>();
            for (int a = 1; a < args.Length; a++)
            {
                switch (args[a].Type)
                {
                    case Variable.VariableType.Boolean:
                        fmtArgs.Add(args[a].ValueBoolean);
                        break;

                    case Variable.VariableType.NoType:
                        throw new Engine.RuntimeException($"Try to use `NoType` to format at param `{a + 1}`");

                    case Variable.VariableType.Number:
                        fmtArgs.Add(args[a].ValueNumber);
                        break;

                    case Variable.VariableType.Object:
                        fmtArgs.Add(args[a].ValueObject);
                        break;

                    case Variable.VariableType.String:
                        fmtArgs.Add(args[a].ValueString);
                        break;
                }
            }

            return new Variable()
            {
                Type = Variable.VariableType.String,
                ValueString = string.Format(commandS, fmtArgs.ToArray())
            };
        }
    }
}
