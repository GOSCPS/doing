/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Engine.cs
 * Content: Engine Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Doing.Engine
{

    /// <summary>
    /// 解析引擎
    /// </summary>
    static class Engine
    {
        private static readonly List<string> IncludedFilePath = new List<string>();

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<(string source,long row,string fileName)> 
            PreProcess(string fileName)
        {
            fileName = fileName.Trim();

            // 不重复Include
            if (IncludedFilePath.Contains(Path.GetFullPath(fileName)))
                return new List<(string source, long row, string fileName)>();

            // 添加到引用路径
            IncludedFilePath.Add(Path.GetFullPath(fileName));


            // 文件不存在则退出
            if (!File.Exists(fileName))
            {
                Tool.Printer.Err($"Build file `{fileName}` not found!");
                throw new FileNotFoundException("File Not Found!", fileName);
            }
            
            List<(string source, long row, string fileName)> source =
                new List<(string source, long row, string fileName)>();

            string[] lines = File.ReadAllLines(fileName);
            long row = 0;

            // 循环
            foreach (var line in lines)
            {
                row++;

                // 以#开头为注释
                // 以##!开头视为预处理
                if (line.Trim().StartsWith("##!"))
                {
                    string preCommand = line.Trim()[3..].TrimStart();

                    // Include指令
                    if (preCommand.StartsWith("Include"))
                    {
                        source.AddRange(PreProcess(preCommand["Include".Length..].Trim()));
                    }

                    // 版本号
                    else if (preCommand.StartsWith("VersionRequired"))
                    {
                        Version required = new Version(preCommand["VersionRequired".Length..].Trim());

                        // 确保版本适用
                        if(Program.DoingVersion.CompareTo(required) == -1)
                        {
                            throw new VersionException("Version too low", required);
                        }
                    }

                    // 扩展
                    // TODO
                    else if (preCommand.StartsWith("UsingSystem"))
                    {
                        // 等价于Load

                    }
                    else if (preCommand.StartsWith("Using"))
                    {
                        // 等价于Load File
                    }

                }
                // 非注释
                // 添加进源
                else if (!line.Trim().StartsWith("#"))
                {
                    source.Add((line.Trim(), row, fileName));
                }
            }

            return source;
        }

        /// <summary>
        /// 启动构建引擎
        /// </summary>
        public static void Start()
        {
            try
            {
                // 解析
                var list = Lexer.Process(PreProcess(Program.RootFile).ToArray());

                // DEBUG
                /*foreach(var token in list)
                {
                    Tool.Printer.Put($"At File `{token.SourceFileName}` Line {token.Line}:");
                    Tool.Printer.Put($"\tToken Type:{token.type:G}");

                    if (token.type == TokenType.number)
                    {
                        Tool.Printer.Put($"\tValue:{token.ValueNumber:G}");
                    }
                    else if (token.type == TokenType.str || token.type == TokenType.identifier)
                    {
                        Tool.Printer.Put($"\tValue:{token.ValueString:G}");
                    }
                }*/

                // 构架全局变量
                foreach(var v in Program.GlobalKeyValuePairs)
                {
                    Utility.Context.GlobalVariableTable.TryAdd(v.Key, new Utility.Variable()
                    {
                        name = v.Key,
                        Type = Utility.Variable.VariableType.String,
                        ValueString = v.Value
                    });
                }

                // 编译
                Emiter.Compile(list);

                Tool.Printer.Ok("Build OK!");
            }
            catch(Exception err)
            {
                Tool.Printer.Err("Build error!");
                Tool.Printer.Err(err.ToString());
                return;
            }
        } 






    }
}
