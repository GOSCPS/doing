//===========================================================
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
using System.Text;
using System.Text.RegularExpressions;
using static Doing.Engine.Target;

namespace Doing.Engine
{
    /// <summary>
    /// 解析器类
    /// </summary>
    static class Parsing
    {
        /// <summary>
        /// Main特殊Target的名称
        /// </summary>
        public const string MainTargetName = "Main";

        #region PreProcess
        /// <summary>
        /// 用于记录Import过的文件
        /// </summary>
        private static readonly List<string> AccessedFile = new();

        /// <summary>
        /// 预处理
        /// </summary>
        /// <returns></returns>
        private static BuildFileInfo[]? PreProcess(
            string fileName, Runspace runspace)
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
                throw new DException.RuntimeException("File not found!",
                    new FileNotFoundException("File Not Found!", fileName));
            }

            // 行
            List<BuildLineInfo> linesInfo =
                new();

            // 文件
            List<BuildFileInfo> filesInfo = new();

            // 当前处理的文件
            BuildFileInfo currentFile = new(new FileInfo(fileName), runspace);

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
                                PreProcess(Path.GetDirectoryName(fileName) + "/" + preCommand["Import".Length..].Trim(), runspace);

                            // 添加到文件列表
                            if (fs != null)
                                filesInfo.AddRange(fs);
                        }
                        catch (Exception)
                        {
                            Tool.Printer.NoFormatErrLine($"File Not Include!\nAt File `{fileName}` Lines {lineNumber}");

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
                            throw new DException.RuntimeException($"Doing version too low!\nAt `{fileName}` Lines {lineNumber}.\n" +
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

        #endregion

        #region ParseTarget
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
            BuildLineInfo definedLine, out (string name, string[] deps) info)
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
            BuildLineInfo defLine, out (string tNamed, string[] tDeps) output)
        {
            if (TryParseTarget(defLine, out output))
            {
                return true;
            }
            else if (TryParseTargetWithDeps(defLine, out output))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ParseFunction
        /// <summary>
        /// 函数定义的正则表达式
        /// </summary>
        private static readonly Regex functionRegex = new(@"^@Function\s+(?<Name>[a-zA-Z0-9-_]+)\s*$");

        private static bool
            TryParseFunctionDefine(BuildLineInfo defLine, out string? funcNamed)
        {
            funcNamed = default;
            if (functionRegex.IsMatch(defLine.Source))
            {
                Match match = functionRegex.Match(defLine.Source);

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

        #endregion

        #region Process
        /// <summary>
        /// 处理
        /// 产生Target
        /// </summary>
        /// 
        /// <param name="buildFile">构建的文件</param>
        public static void Process(
            BuildFileInfo buildFile, Runspace runspace)
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
                else if (TryParseTargetDefine(buildFile.AllLines[lineNumber], out (string name, string[] deps) info))
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
                    Target target = new(definedLine, runspace, sourceCode.ToString(), info.name, info.deps);

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
                else if (TryParseFunctionDefine(buildFile.AllLines[lineNumber], out string? funcName))
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
                        else if (buildFile.AllLines[lineNumber].Source.Trim() == "@EndFunction")
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
                    Function function = new(definedLine, runspace, funcName!, sourceCode.ToString());

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
            if (allTargets.TryGetValue("Main", out Target? mainTarget))
            {
                allTargets.Remove("Main");

                buildFile.MainTarget = mainTarget;
            }

            // 移动Init
            if(allTargets.TryGetValue("Init",out Target? initTarget))
            {
                allTargets.Remove("Init");

                buildFile.InitTarget = initTarget;
            }

            // 设置构建文件AllTarget
            buildFile.AllTargets = allTargets.Values.ToArray();
            buildFile.AllFunction = allFunctions.Values.ToArray();

            // 检查依赖
            CheckDepends(buildFile);
        }

        #endregion

        #region CheckDeps
        /// <summary>
        /// 检查依赖
        /// </summary>
        /// <param name="buildFile"></param>
        private static void CheckDepends(BuildFileInfo buildFile)
        {
            // Main禁止依赖
            if (buildFile.MainTarget != null)
            {
                if (buildFile.MainTarget.Deps.Length != 0)
                {
                    throw new DException.RuntimePositionException($"The target `Main` isn't able to have depends!",
                        buildFile.MainTarget.DefineLine);
                }
            }

            // Init禁止依赖
            if (buildFile.InitTarget != null)
            {
                if (buildFile.InitTarget.Deps.Length != 0)
                {
                    throw new DException.RuntimePositionException($"The target `Init` isn't able to have depends!",
                        buildFile.InitTarget.DefineLine);
                }
            }

            // Target禁止依赖Main和Init
            foreach (var target in buildFile.AllTargets)
            {
                foreach (var deps in target.Deps)
                {
                    if (deps == MainTargetName || deps == "Init")
                    {
                        throw new DException.RuntimePositionException($"The target `{target.Name}` isn't able to depend for `Main` or `Init`!",
                        target.DefineLine);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 加载文件到运行空间，不处理依赖
        /// </summary>
        /// 
        /// <param name="runspace">运行空间</param>
        /// <param name="fileName">文件名称</param>
        public static void LoadFileToRunspace(
            Runspace runspace,
            string fileName)
        {
            var info = ParseFile(fileName, runspace);

            // 添加到Runspace
            foreach (var file in info.Item1)
            {
                runspace.SourceFile.TryAdd(file.SourceFile.FullName, file);
            }
            foreach (var target in info.Item2)
            {
                runspace.AllTarget.TryAdd(target.Name, target);
            }
            foreach (var function in info.Item3)
            {
                runspace.AllFunction.TryAdd(function.Name, function);
            }

            return;
        }

        #region ParseFile
        /// <summary>
        /// 解析文件
        /// </summary>
        /// <param name="runspace">命名空间，将会绑定</param>
        /// <param name="fileName">要处理的文件名称</param>
        public static (BuildFileInfo[], Target[], Function[]) ParseFile(
            string fileName, Runspace runspace)
        {
            // 获取文件列表
            BuildFileInfo[] fileInfo = PreProcess(fileName, runspace)!;

            // 导出Target
            Dictionary<string, Target> targetTable = new();

            // 负责检查重名Target
            foreach (var f in fileInfo)
            {
                // 解析文件
                Process(f, runspace);
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

            return (fileInfo, targetTable.Values.ToArray(), functionTable.Values.ToArray());
        }
        #endregion

        #region MakeRunspaceDeps
        /// <summary>
        /// 处理依赖
        /// </summary>
        public static (Queue<Target> first,Queue<Target> last) MakeRunspaceDeps(
            Runspace runspace,
            List<string> aimTargets)
        {
            // 用户未指定Aim
            if (aimTargets.Count == 0)
            {
                throw new ArgumentException("The aim target count is zero!",nameof(aimTargets));
            }

            // 查找依赖
            foreach (var t in aimTargets)
            {
                if (runspace.AllTarget.TryGetValue(t, out Target? value))
                {
                    runspace.AimTargetQueue.Enqueue(value);
                }
                else throw new ArgumentException($"The aim target `{t}` not found!",nameof(aimTargets));
            }

            // Init和Main优先构建
            Queue<Target> first = new();

            // 检查依赖
            var depends = Algorithm.Topological.Sort(runspace.AimTargetQueue.ToArray(), runspace.AllTarget.Values.ToArray());

            // 添加Init
            foreach(var file in runspace.SourceFile)
            {
                if(file.Value.InitTarget != null)
                {
                    first.Enqueue(file.Value.InitTarget);
                }
            }

            // 查找&添加Main
            foreach (var target in first)
            {
                if (target.DefineLine.Position.MainTarget != null
                    && target.DefineLine.Position.IsMainBuilt == false)
                {
                    target.DefineLine.Position.IsMainBuilt = true;
                }
            }

            // 其他Target后执行
            Queue<Target> last = new();
            // 添加到Worker
            foreach (var target in depends)
            {
                last.Enqueue(target);
            }

            // first先执行
            // last其次
            return (first,last);
        }
        #endregion
    }
}
