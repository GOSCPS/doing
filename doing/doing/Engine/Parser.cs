using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Doing.Engine
{
    /// <summary>
    /// 行信息
    /// </summary>
    public class BuildLineInfo
    {
        public string Source = "";
        public int LineNumber = 0;
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    public class BuildFileInfo
    {
        public string FileName = "";
        public BuildLineInfo[] Lines = Array.Empty<BuildLineInfo>();
    }

    /// <summary>
    /// 构建模块信息
    /// </summary>
    public class BuildModuleInfo
    {
        public BuildFileInfo SourceFile = new();
        public Target[] Targets = Array.Empty<Target>();
        public Target? MainTarget = null;
    }


    /// <summary>
    /// 解析器
    /// </summary>
    class Parser
    {
        /// <summary>
        /// 检查名称是否符合命名规则
        /// </summary>
        /// 
        /// <param name="name">要检查的字符串</param>
        /// <param name="err">第一个错误的字符，如果字符串长度为0则为\0</param>
        /// 
        /// <returns>符合返回true，否则false</returns>
        public static bool CheckName(string name,out char err)
        {
            err = '\0';

            if (name.Length == 0)
                return false;

            if (char.IsDigit(name[0]))
            {
                err = name[0];
                return false;
            }

            for (int c = 0; c < name.Length; c++)
            {
                if (!char.IsLetterOrDigit(name[c]))
                {
                    err = name[c];
                    return false;
                }
            }

            return true;
        }

        private static readonly List<string> IncludedFilePath = new();

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BuildFileInfo[]
            PreProcess(string fileName)
        {
            fileName = fileName.Trim();
            fileName = Path.GetFullPath(fileName);

            // 不重复Import
            if (IncludedFilePath.Contains((fileName)))
                return Array.Empty<BuildFileInfo>();

            // 添加到引用路径
            IncludedFilePath.Add(fileName);


            // 文件不存在则退出
            if (!File.Exists(fileName))
            {
                Tool.Printer.ErrLine($"Import file `{fileName}` not found!");
                throw new FileNotFoundException("File Not Found!", fileName);
            }

            // 添加文件
            List<BuildLineInfo> linesInfo =
                new();

            List<BuildFileInfo> filesInfo = new();

            string[] lines = File.ReadAllLines(fileName);
            int row = 0;

            // 循环遍历文件
            foreach (var line in lines)
            {
                row++;

                // 以#开头为注释
                // 以##!开头视为预处理
                if (line.Trim().StartsWith("##!"))
                {
                    string preCommand = line.Trim()[3..].TrimStart();

                    // Include指令
                    if (preCommand.StartsWith("Import"))
                    {
                        // 捕获文件缺失异常
                        try
                        {
                            // 当前文件路径/Import文件路径 
                            filesInfo.AddRange(
                                PreProcess(Path.GetDirectoryName(fileName) + "/" + preCommand["Import".Length..].Trim()));
                        }
                        catch (FileNotFoundException)
                        {
                            Tool.Printer.ErrLine($"File Not Include!\nAt File `{fileName}` Lines {row}");
                            throw;
                        }
                    }

                    // 版本号
                    else if (preCommand.StartsWith("VersionRequired"))
                    {
                        Version required = new(preCommand["VersionRequired".Length..].Trim());

                        // 当前版本早于required
                        if (Program.DoingVersion.CompareTo(required) == -1)
                        {
                            // 报错
                            throw new Exception.RuntimeException($"Doing version too low!\nAt File `{fileName}` Lines {row}.\n" +
                                $"Required {required} at least!");
                        }
                    }


                    
                }
                // 添加进源文件
                linesInfo.Add(new BuildLineInfo() { Source = line,LineNumber = row });

            }

            // 添加当前处理的源文件
            filesInfo.Add(new BuildFileInfo() { FileName = fileName, Lines = linesInfo.ToArray() });

            return filesInfo.ToArray();
        }

        /// <summary>
        /// 处理构建文件
        /// </summary>
        /// 
        /// <param name="file">文件名称</param>
        /// 
        /// <returns>模块</returns>
        public static BuildModuleInfo ProcessFile(BuildFileInfo file) 
        {
            // Target NAME : DEPS
            // PowerShell Code
            // EndTarget

            BuildModuleInfo module = new();
            List<string> targetsName = new();
            List<Target> targets = new();
            Target? main = null;

            for(int ptr=0;ptr < file.Lines.Length; ptr++)
            {
                // 检查到Target
                if (file.Lines[ptr].Source.Trim().StartsWith("Target"))
                {
                    string nameDepsPair = file.Lines[ptr].Source["Target".Length..].Trim();
                    Target target = new();

                    // 含依赖
                    if (nameDepsPair.Contains(':'))
                    {
                        string named = nameDepsPair[0..(nameDepsPair.IndexOf(':'))].Trim();
                        string[] deps = Regex.Split(nameDepsPair[(nameDepsPair.IndexOf(':') + 1)..], @"\s+")
                            .SkipWhile(
                            (str) => { if (str.Trim().Length == 0) return true; else return false; }).ToArray();

                        // 检查名称
                        if (!CheckName(named, out char err))
                        {
                            if (err == '\0')
                                throw new Exception.RuntimeException("The target name is empty!",
                                    file, file.Lines[ptr]);

                            if (char.IsDigit(err))
                                throw new Exception.RuntimeException("The target name begin with digit!",
                                    file, file.Lines[ptr]);

                            throw new Exception.RuntimeException($"The target name contains illegal characters `{err}`!",
                                    file, file.Lines[ptr]);
                        }

                        // 检查依赖名称
                        foreach (var dep in deps)
                        {
                            if (!CheckName(dep, out err))
                            {
                                if (err == '\0')
                                    throw new Exception.RuntimeException($"The target depend `{dep}` is empty!",
                                        file, file.Lines[ptr]);

                                if (char.IsDigit(err))
                                    throw new Exception.RuntimeException($"The target depend `{dep}` begin with digit!",
                                        file, file.Lines[ptr]);

                                throw new Exception.RuntimeException($"The target depend `{deps}` contains illegal characters `{err}`!",
                                        file, file.Lines[ptr]);
                            }
                        }

                        target.Name = named;
                        target.Deps = deps;
                    }
                    // 不包含依赖
                    else
                    {
                        if (!CheckName(nameDepsPair, out char err))
                        {
                            if (err == '\0')
                                throw new Exception.RuntimeException("The target name is empty!",
                                    file, file.Lines[ptr]);

                            if (char.IsDigit(err))
                                throw new Exception.RuntimeException("The target name begin with digit!",
                                    file, file.Lines[ptr]);

                            throw new Exception.RuntimeException($"The target name contains illegal characters `{err}`!",
                                    file, file.Lines[ptr]);
                        }

                        target.Name = nameDepsPair;
                    }

                    // 获取源代码
                    target.FileName = file.FileName;
                    target.StartLine = ptr+1;
                    ptr++;
                    StringBuilder sourceBuilder = new();

                    while (true)
                    {
                        if (ptr >= file.Lines.Length)
                            throw new Exception.RuntimeException("The target not match end!", file, file.Lines[^1]);

                        else if (file.Lines[ptr].Source.Trim() == "EndTarget")
                        {
                            break;
                        }
                        else
                        {
                            sourceBuilder.Append(file.Lines[ptr].Source + "\n");
                            ptr++;
                        }
                    }

                    target.SourceCode = sourceBuilder.ToString();

                    // 添加Main
                    if(target.Name == "Main")
                    {
                        if (main != null)
                            throw new Exception.RuntimeException("One file only can define a `Main`!", file, file.Lines[target.StartLine]);

                        else main = target;
                    }
                    // 名称重复则不添加
                    else if (targetsName.Contains(target.Name))
                    {
                        if (main != null)
                            throw new Exception.RuntimeException($"The target `{target.Name}` is defined!", file, file.Lines[target.StartLine]);
                    }
                    else
                    {
                        targets.Add(target);
                        targetsName.Add(target.Name);
                    }
                }
                // 非注释和空白行
                // 未知的操作
                else if (!((file.Lines[ptr].Source.Trim().StartsWith('#')) || (file.Lines[ptr].Source.Trim().Length == 0)))
                {
                    throw new Exception.RuntimeException("Unknown statement!", file,file.Lines[ptr]);
                }
            }

            module.MainTarget = main;
            module.SourceFile = file;
            module.Targets = targets.ToArray();

            return module;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <returns></returns>
        public static void Parsing()
        {
            var files = PreProcess(Program.BuildFile).ToArray();

            // 解析依赖
            List<BuildModuleInfo> moduleInfos = new();
            List<Target> targetList = new();

            foreach(var f in files)
            {
                moduleInfos.Add(ProcessFile(f));
            }

            // 设置Module
            Runner.Modules = moduleInfos.ToArray();

            // 添加Target
            foreach(var module in Runner.Modules)
            {
                foreach(var target in module.Targets)
                {
                    // 不重复添加
                    if (Runner.TargetList.TryGetValue(target.Name,out (BuildModuleInfo,Target) output))
                    {
                        throw new Exception.RuntimeException($"The target `{target.Name}` is defined " +
                            $"at `{output.Item1.SourceFile.FileName}` " +
                            $"Line {output.Item1.SourceFile.Lines[output.Item2.StartLine]}!",
                            module.SourceFile,module.SourceFile.Lines[target.StartLine]);
                    }
                    else if (!Runner.TargetList.TryAdd(target.Name, (module, target)))
                    {
                        throw new Exception.RuntimeException("Add target fail down!",
                        module.SourceFile, module.SourceFile.Lines[target.StartLine]);
                    }
                    else
                    {
                        targetList.Add(target);
                    }
                }
            }

            // 获取目标
            List<Target> aims = new();

            foreach(var str in Program.AimTargets)
            {
                if (Runner.TargetList.TryGetValue(str, out (BuildModuleInfo, Target) result))
                {
                    aims.Add(result.Item2);
                }
                else
                {
                    throw new Exception.RuntimeException($"Not found target `{str}` to build");
                }
            }

            // 拓扑排序
            foreach (var t in Algorithm.Topological.Sort(aims.ToArray(), targetList.ToArray()))
                Runner.AimTargets.Enqueue(t);

            return;
        } 










    }
}
