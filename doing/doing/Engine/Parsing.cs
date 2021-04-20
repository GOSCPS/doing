﻿//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Parsing.cs
// Content: Parsing Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Doing.Engine.Target;

namespace Doing.Engine
{
    /// <summary>
    /// 解析器类
    /// </summary>
    static class Parsing
    {
        /// <summary>
        /// 用于记录Import过的文件
        /// </summary>
        private static readonly List<string> AccessedFile = new();

        /// <summary>
        /// 预处理
        /// </summary>
        /// <returns></returns>
        private static BuildFileInfo[]? PreProcess(string fileName)
        {
            // 取绝对路径
            fileName = Path.GetFullPath(fileName.Trim());

            // Import过则忽略
            if (AccessedFile.Contains(fileName))
            {
                return null;
            }
            // 添加到已Import列表
            else AccessedFile.Add(fileName);

            // 文件不存在则退出
            if (!File.Exists(fileName))
            {
                Tool.Printer.NoFormatErrLine($"Import file `{fileName}` not found!");
                throw new FileNotFoundException("File Not Found!", fileName);
            }

            // 行
            List<BuildLineInfo> linesInfo =
                new();

            // 文件
            List<BuildFileInfo> filesInfo = new();

            // 当前处理的文件
            BuildFileInfo currentFile = new(new FileInfo(fileName));

            // 总行
            string[] lines = File.ReadAllLines(fileName);
            int lineNumber = 0;

            // 循环遍历文件
            foreach (var line in lines)
            {
                lineNumber++;

                // 以#开头为注释
                // 以##!开头视为预处理
                if (line.Trim().StartsWith("##!"))
                {
                    string preCommand = line.Trim()[3..].TrimStart();

                    // Import指令
                    if (preCommand.StartsWith("Import"))
                    {
                        // 捕获文件缺失异常
                        try
                        {
                            // 当前(参数)文件所在目录/Import文件路径 
                            var fs = 
                                PreProcess(Path.GetDirectoryName(fileName) + "/" + preCommand["Import".Length..].Trim());

                            // 添加到文件列表
                            if (fs != null)
                                filesInfo.AddRange(fs);
                        }
                        catch (FileNotFoundException f)
                        {
                            Tool.Printer.NoFormatErrLine($"File Not Include!\nAt File `{fileName}` Lines {lineNumber}");
                            Tool.Printer.NoFormatErrLine($"The miss file name is:`{f.FileName}`");
                            throw;
                        }
                    }

                    // 检查版本号
                    else if (preCommand.StartsWith("VersionRequired"))
                    {
                        Version required = new(preCommand["VersionRequired".Length..].Trim());

                        // 当前版本早于required
                        if (Program.DoingVersion.CompareTo(required) < 0)
                        {
                            // 退出
                            throw new DException.RuntimeException($"Doing version too low!\nAt File `{fileName}` Lines {lineNumber}.\n" +
                                $"Required {required} at least!");
                        }
                    }

                }
                // 添加进源文件
                linesInfo.Add(new BuildLineInfo(currentFile) { Source = line, LineNumber = lineNumber });
            }

            // 添加当前处理的源文件
            currentFile.AllLines = linesInfo.ToArray();
            filesInfo.Add(currentFile);

            return filesInfo.ToArray();
        }

        // Target定义的正则表达式
        private static readonly             
            Regex targetRegex = new(@"^@Target\s+(?<Name>[a-zA-Z0-9-_\u4e00-\u9fa5]+)\s*$");
        private static readonly 
            Regex targetRegexWithDeps = new(@"^@Target\s+(?<Name>[a-zA-Z0-9-_\u4e00-\u9fa5]+)\s*:\s*(?<Deps>[^:]+)$");

        /// <summary>
        /// 尝试解析Target
        /// </summary>
        /// <param name="definedLine"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool TryParseTarget(
            BuildLineInfo definedLine,out (string name, string[] deps) info)
        {
            // 匹配到Target
            if (targetRegex.IsMatch(definedLine.Source))
            {
                Match match = targetRegex.Match(definedLine.Source);

                // only one matches
                string named = match.Groups["Name"].Value.Trim();

                // 检查名称是否合法
                if (!Utility.CheckName(named, out _))
                    throw
                        new DException.RuntimePositionException("The target name is unlawful!",
                        definedLine);

                //target = new(buildFile.AllLines[lineNumber])
                //{
                //Name = named,
                //};
                info = new(named, Array.Empty<string>());

                // 错误❌
                // regex有多个match
                if (match.NextMatch().Success)
                    throw
                        new DException.RuntimePositionException("The regex match should only one!",
                        definedLine);

                return true;
            }
            else
            {
                info = new("", Array.Empty<string>());
                return false;
            }
        }

        /// <summary>
        /// 尝试解析带依赖的Target
        /// </summary>
        /// <param name="definedLine"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool TryParseTargetWithDeps(
            BuildLineInfo definedLine, out (string name, string[] deps) info)
        {
            // 匹配到有依赖的Target
            if (targetRegexWithDeps.IsMatch(definedLine.Source))
            {
                Match match = targetRegexWithDeps.Match(definedLine.Source);

                // only one matches
                string named = match.Groups["Name"].Value.Trim();

                // 检查名称是否合法
                if (!Utility.CheckName(named, out _))
                    throw
                        new DException.RuntimePositionException($"The target name `{named}` is unlawful!",
                        definedLine);

                // 获取依赖
                string[] deps = Regex.Split(match.Groups["Deps"].Value, @"\s+")
                    // 跳过空白
                    .SkipWhile((str) =>
                    {
                        if (str.Trim().Length == 0) return true;
                        else return false;
                    }).ToArray();

                // 检查依赖名称是否合法
                foreach (var depend in deps)
                {
                    // 检查名称是否合法
                    if (!Utility.CheckName(depend, out _))
                        throw
                            new DException.RuntimePositionException($"The target depend name `{depend}` is unlawful!",
                            definedLine);
                }

                //target = new(buildFile.AllLines[lineNumber])
                //{
                //    Name = named,
                //    Deps = deps
                //};
                info = new(named, deps);

                // 错误❌
                // regex有多个match
                if (match.NextMatch().Success)
                    throw
                        new DException.RuntimePositionException("The regex match should only one!",
                        definedLine);

                return true;
            }
            else
            {
                info = new("", Array.Empty<string>());
                return false;
            }
        }

        /// <summary>
        /// 尝试解析Target定义
        /// </summary>
        /// <param name="defLine"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool TryParseTargetDefine(
            BuildLineInfo defLine,out (string tNamed, string[] tDeps) output)
        {
            if(TryParseTarget(defLine,out output))
            {
                return true;
            }
            else if(TryParseTargetWithDeps(defLine,out output))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 函数定义的正则表达式
        /// </summary>
        private static readonly Regex functionRegex = new(@"@Function\s+(?<Name>[])\s+");

        private static bool
            TryParseFunctionDefine(BuildLineInfo defLine,out string? funcNamed)
        {
            funcNamed = default;
            if (functionRegex.IsMatch(defLine.Source))
            {
                Match match = targetRegex.Match(defLine.Source);

                // only one matches
                string named = match.Groups["Name"].Value.Trim();

                // 检查名称是否合法
                if (!Utility.CheckName(named, out _))
                    throw
                        new DException.RuntimePositionException("The function name is unlawful!",
                        defLine);
                
                // 赋值名称
                funcNamed = named;

                // 错误❌
                // regex有多个match
                if (match.NextMatch().Success)
                    throw
                        new DException.RuntimePositionException("The regex match should only one!",
                        defLine);

                else return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 处理
        /// 产生Target
        /// </summary>
        /// 
        /// <param name="buildFile">构建的文件</param>
        public static void Process(BuildFileInfo buildFile)
        {
            // 查找到的Target和Function
            Dictionary<string, Target> allTargets = new();
            Dictionary<string, Function> allFunctions = new();

            // 遍历行
            for (int lineNumber = 0; lineNumber < buildFile.AllLines.Length; lineNumber++)
            {
                // 跳过空行和注释
                if (buildFile.AllLines[lineNumber].Source.Trim().Length == 0)
                    continue;
                else if (buildFile.AllLines[lineNumber].Source.Trim().StartsWith('#'))
                    continue;
                // Target定义
                else if(TryParseTargetDefine(buildFile.AllLines[lineNumber],out (string name, string[] deps) info))
                {
                    BuildLineInfo definedLine = buildFile.AllLines[lineNumber];

                    // 收集target源代码
                    lineNumber++;
                    StringBuilder sourceCode = new();

                    while (true)
                    {
                        // 意外的行尾
                        if (lineNumber >= buildFile.AllLines.Length)
                        {
                            throw new DException.RuntimePositionException($"Miss keyword `@EndTarget`!",
                                buildFile.AllLines[^1]);
                        }
                        // 结尾
                        else if (buildFile.AllLines[lineNumber].Source.Trim() == "@EndTarget")
                        {
                            break;
                        }
                        // 代码
                        else
                        {
                            sourceCode.Append(buildFile.AllLines[lineNumber].Source + '\n');
                            lineNumber++;
                        }
                    }

                    // 添加Target
                    Target target = new(definedLine, sourceCode.ToString(), info.name, info.deps);

                    if (allTargets.TryGetValue(target!.Name, out Target? def))
                    {
                        // 重定义!
                        throw new DException.RuntimePositionException(
                                $"The target is defined At {def.DefineLine.Position.SourceFile.FullName} Lines {def.DefineLine.LineNumber}!\n",
                                target.DefineLine);
                    }
                    // 添加到列表
                    else allTargets.Add(target!.Name, target);
                }
                // Function定义
                else if(TryParseFunctionDefine(buildFile.AllLines[lineNumber], out string? funcName))
                {
                    BuildLineInfo definedLine = buildFile.AllLines[lineNumber];

                    // 收集function源代码
                    lineNumber++;
                    StringBuilder sourceCode = new();

                    while (true)
                    {
                        // 意外的行尾
                        if (lineNumber >= buildFile.AllLines.Length)
                        {
                            throw new DException.RuntimePositionException($"Miss keyword `@EndTarget`!",
                                buildFile.AllLines[^1]);
                        }
                        // 结尾
                        else if (buildFile.AllLines[lineNumber].Source.Trim() == "@EndTarget")
                        {
                            break;
                        }
                        // 代码
                        else
                        {
                            sourceCode.Append(buildFile.AllLines[lineNumber].Source + '\n');
                            lineNumber++;
                        }
                    }

                    // 添加Target
                    Function function = new(definedLine, sourceCode.ToString(), funcName!);

                    if (allFunctions.TryGetValue(function!.Name, out Function? def))
                    {
                        // 重定义!
                        throw new DException.RuntimePositionException(
                                $"The target is defined At {def.DefineLine.Position.SourceFile.FullName} Lines {def.DefineLine.LineNumber}!\n",
                                function.DefineLine);
                    }
                    // 添加到列表
                    else allFunctions.Add(function!.Name, function);
                }
                // 未知语句
                else
                {
                    throw new DException.RuntimePositionException("Unknown sentence!", buildFile.AllLines[lineNumber]);
                }
            }

            // 移动Main
            if(allTargets.TryGetValue("Main",out Target? mainTarget))
            {
                allTargets.Remove("Main");

                // 禁止依赖
                if (mainTarget.Deps.Length != 0)
                    throw new DException.RuntimePositionException($"The target `Main` isn't able to have depends!",
                        mainTarget.DefineLine);

                buildFile.MainTarget = mainTarget;
            }

            // 设置构建文件AllTarget
            buildFile.AllTargets = allTargets.Values.ToArray();
            buildFile.AllFunction = allFunctions.Values.ToArray();
        }


        public static void ParseMain()
        {
            var info = Parse(Program.BuildFile);

            // 添加到Runspace
            foreach (var file in info.Item1)
            {
                Runspace.SourceFile.TryAdd(file.SourceFile.FullName, file);
            }
            foreach (var target in info.Item2)
            {
                Runspace.AllTarget.TryAdd(target.Name, target);
            }
            foreach(var function in info.Item3)
            {
                Runspace.AllFunction.TryAdd(function.Name, function);
            }

            // 处理依赖
            MakeRunspaceDeps();
        }

        /// <summary>
        /// 解析
        /// </summary>
        public static (BuildFileInfo[],Target[],Function[]) Parse(string fileName)
        {
            // 获取文件列表
            BuildFileInfo[] fileInfo = PreProcess(fileName)!;

            // 导出Target
            Dictionary<string, Target> targetTable = new();

            // 负责检查重名Target
            foreach (var f in fileInfo)
            {
                Process(f);
                var targets = f.AllTargets;

                // 检查重名Target
                foreach (var t in targets)
                {
                    if (targetTable.TryGetValue(t.Name, out Target? def))
                    {
                        // 重定义!
                        throw new DException.RuntimePositionException(
                            $"The target is defined At {def.DefineLine.Position.SourceFile.FullName} Lines {def.DefineLine.LineNumber}!\n",
                            t.DefineLine);
                    }
                    else targetTable.Add(t.Name, t);
                }
            }

            // 导出Function
            Dictionary<string, Function> functionTable = new();

            // 负责检查重名Function
            foreach (var f in fileInfo)
            {
                Process(f);
                var functions = f.AllFunction;

                // 检查重名Function
                foreach (var func in functions)
                {
                    if (functionTable.TryGetValue(func.Name, out Function? def))
                    {
                        // 重定义!
                        throw new DException.RuntimePositionException(
                            $"The target is defined At {def.DefineLine.Position.SourceFile.FullName} Lines {def.DefineLine.LineNumber}!\n",
                            func.DefineLine);
                    }
                    else functionTable.Add(func.Name, func);
                }
            }

            return (fileInfo, targetTable.Values.ToArray(),functionTable.Values.ToArray());
        }

        /// <summary>
        /// 处理依赖
        /// </summary>
        public static void MakeRunspaceDeps()
        {
            // 用户未指定
            // 添加Default Target
            if(Program.AimTargets.Count == 0)
            {
                Tool.Printer.WarnLine("Not specified target.build `Default`.");
                Program.AimTargets.Add("Default");
            }

            // 查找依赖
            List<Target> aims = new();
            foreach(var t in Program.AimTargets)
            {
                if (Runspace.AllTarget.TryGetValue(t, out Target? value))
                {
                    aims.Add(value);
                }
                else throw new DException.RuntimeException($"The aim target `{t}` not found!");
            }

            // 添加依赖
            foreach(var t in Algorithm.Topological.Sort(aims.ToArray(), Runspace.AllTarget.Values.ToArray()))
            {
                if (!Runspace.AimTarget.TryAdd(t.Name, t))
                {
                    throw new DException.RuntimeException($"Add aim target `{t.Name}` error!");
                }
            }

            // 查找Main
            foreach(var target in Runspace.AimTarget.Values)
            {
                if(target.DefineLine.Position.MainTarget != null
                    && target.DefineLine.Position.IsMainBuilt == false)
                {
                    target.DefineLine.Position.IsMainBuilt = true;
                    Worker.AddTarget(target.DefineLine.Position.MainTarget);
                }
            }

            // 添加到Worker
            foreach(var target in Runspace.AimTarget)
            {
                Worker.AddTarget(target.Value);
            }

            return;
        }
    }
}