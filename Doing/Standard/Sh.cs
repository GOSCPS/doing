﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Sh.cs
 * Content: Sh Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;


namespace Doing.Standard
{
    /// <summary>
    /// 执行shell命令
    /// </summary>
    class Sh : Engine.Utility.Function
    {
        public override Variable Execute(Context callerContext, Variable[] args)
        {
            if (args.Length != 1)
                throw new Engine.RuntimeException("Need one string param!");

            if (args[0].Type != Variable.VariableType.String)
                throw new Engine.RuntimeException("Param type isn't string!");

            string commandS = args[0].ValueString;
            StringBuilder command;
            bool replaced = true;

            // 替换变量
            for (int count = 0;count < 1024 && replaced;count++)
            {
                replaced = false;
                command = new StringBuilder();

                for (int ptr=0;ptr < commandS.Length; ptr++)
                {

                    // ${}视为变量
                    if(commandS[ptr] == '$')
                    {
                        // $$ 视为$
                        ptr++;

                        if (ptr >= commandS.Length)
                            throw new Engine.RuntimeException("Miss token `$` or `{`!");

                        if (commandS[ptr] == '$')
                            command.Append('$');

                        // 获取变量名
                        else
                        {
                            // 有变量替换
                            replaced = true;

                            // 检查{
                            ptr++;
                            if (ptr >= commandS.Length)
                                throw new Engine.RuntimeException("Miss token `{`!");

                            if(commandS[ptr] != '{')
                                throw new Engine.RuntimeException("Expect token `{`!");

                            ptr++;

                            // 获取变量名
                            StringBuilder varName = new StringBuilder();

                            while (true)
                            {
                                if (ptr >= commandS.Length)
                                    throw new Engine.RuntimeException("Miss token `{`!");

                                else if (commandS[ptr] == '}')
                                    break;

                                else varName.Append(commandS[ptr]);
                            }

                            // 读取变量
                            if(!Context.TryGetVariable(callerContext,varName.ToString(),out Variable? variable)){
                                throw new Engine.RuntimeException($"Variable `{varName.ToString()}` Not Found!");
                            }
                            else
                            {
                                // 仅支持string
                                if (variable!.Type != Variable.VariableType.String)
                                    throw new Engine.RuntimeException($"Variable `{varName.ToString()}`'s Type isn't String!");

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

            // 启动进程
            Process shell = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "sh",
                    Arguments = $"-c \"{commandS}\"",
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    UseShellExecute = false
                }
            };
            Tool.Printer.Put($"{shell.StartInfo.FileName} {shell.StartInfo.Arguments}");
            shell.Start();
            shell.WaitForExit();

            if (shell.ExitCode != 0)
                throw new Engine.RuntimeException("Shell Command Not Return 0!");

            return new Variable()
            {
                Type = Variable.VariableType.Number,
                ValueNumber = shell.ExitCode
            };
        }
    }
}