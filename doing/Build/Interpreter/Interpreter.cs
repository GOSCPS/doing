/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Interpreter.cs
 * Content: Interpreter Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;

namespace doing.Build.Interpreter
{
    /// <summary>
    /// 代码上下文信息
    /// </summary>
    public struct LineInfo
    {
        public string FileName { get; set; }
        public int Line { get; set; }

        public override string ToString()
        {
            return $"At file `{FileName}` line `{Line}`";
        }
    }

    /// <summary>
    /// 解释器
    /// </summary>
    public class Interpreter
    {

        /// <summary>
        /// 中间件
        /// deps-target
        /// </summary>
        private static List<ValueTuple<string[], Target>> objects
            = new List<(string[], Target)>();

        /// <summary>
        /// 解析预处理后的代码
        /// </summary>
        /// <returns></returns>
        private static void Parse()
        {
            for (int ptr = 0; ptr < Preprocessor.AllLines.Count; ptr++)
            {
                //预处理后的源文件
                //No include & using
                //No empty line
                Tool.StringIterator stringIterator =
                    new Tool.StringIterator(Preprocessor.AllLines[ptr].Item2);

                //宏调用
                if (stringIterator.Current == '\\')
                {
                    stringIterator.Next();
                    //\"MacroName"{"Macro Param"}
                    //读取宏名
                    string macroName = stringIterator.ReadNextString();
                    string macroParam = "";


                    if (macroName == null)
                        throw new
                            Exception.GrammaticalException
                            ("Error macro name", Preprocessor.AllLines[ptr].Item1);

                    //{
                    if (stringIterator.CanRead() && stringIterator.Current == '{')
                    {
                        stringIterator.Next();
                        //读取参数
                        macroParam = stringIterator.ReadNextString();

                        if (macroParam == null)
                            throw new
                            Exception.GrammaticalException
                            ("Error macro param", Preprocessor.AllLines[ptr].Item1);

                        //}
                        if (!(stringIterator.CanRead() && stringIterator.Current == '}'))
                            throw new
                                Exception.GrammaticalException
                                ("Miss token `}`", Preprocessor.AllLines[ptr].Item1);

                        stringIterator.Next();
                        stringIterator.SkipSpace();
                    }
                    else throw new
                            Exception.GrammaticalException
                            ("Miss token `{`", Preprocessor.AllLines[ptr].Item1);

                    //末尾有其他字符
                    if (stringIterator.CanRead())
                        throw new
                            Exception.GrammaticalException
                            ("Remaining characters after macro", Preprocessor.AllLines[ptr].Item1);

                    //当场调用
                    var result = MacroManager.CallMacro(macroName, macroParam, null);

                    //执行错误
                    if (result == null || result == false)
                        throw new
                            Exception.GrammaticalException
                            ("Macro run return error", Preprocessor.AllLines[ptr].Item1);

                    continue;
                }
                //非宏
                //target
                else
                {
                    string keyword = stringIterator.ReadNextWord();

                    //target NAME : DEPS
                    if (keyword != "target")
                        throw new
                            Exception.GrammaticalException
                            ("Unknow key word", Preprocessor.AllLines[ptr].Item1);

                    stringIterator.SkipSpace();

                    //读取名称
                    if (!(stringIterator.CanRead() && stringIterator.Current == '"'))
                        throw new
                            Exception.GrammaticalException
                            ("Target name format error", Preprocessor.AllLines[ptr].Item1);
                    string targetName = stringIterator.ReadNextString();

                    if (targetName == null)
                        throw new
                            Exception.GrammaticalException
                            ("Target name format error", Preprocessor.AllLines[ptr].Item1);

                    List<string> depsName = new List<string>();

                    //判断是否有依赖
                    stringIterator.SkipSpace();
                    if (stringIterator.CanRead() && stringIterator.Current == ':')
                    {
                        stringIterator.Next();
                        stringIterator.SkipSpace();

                        //读取依赖
                        while (true)
                        {
                            if (!stringIterator.CanRead())
                                break;
                            else if (stringIterator.Current != '"')
                                throw new
                                    Exception.GrammaticalException
                                    ("Target deps format error", Preprocessor.AllLines[ptr].Item1);

                            var dep = stringIterator.ReadNextString();
                            if (dep == null)
                                throw new
                                   Exception.GrammaticalException
                                   ("Target deps format error", Preprocessor.AllLines[ptr].Item1);

                            else depsName.Add(dep);
                            stringIterator.SkipSpace();
                        }

                    }
                    else if (stringIterator.CanRead())
                        throw new
                            Exception.GrammaticalException
                            ("Target dep format error", Preprocessor.AllLines[ptr].Item1);

                    //储存代码
                    List<ValueTuple<string, LineInfo>> codes
                        = new List<ValueTuple<string, LineInfo>>();

                    ptr++;

                    //添加代码
                    while (true)
                    {
                        if (ptr >= Preprocessor.AllLines.Count)
                            throw new Exception.GrammaticalException
                                ("Miss target end token `end`", Preprocessor.AllLines[ptr].Item1);

                        if ((Preprocessor.AllLines[ptr].Item2 == "end"))
                        {
                            break;
                        }

                        codes.Add((Preprocessor.AllLines[ptr].Item2, Preprocessor.AllLines[ptr].Item1));
                        ptr++;
                    }

                    //制作objects
                    objects.Add((depsName.ToArray(), new Target()
                    {
                        Codes = codes.ToArray(),
                        Deps = Array.Empty<Target>(),
                        Name = targetName
                    }));
                }
            }
            return;
        }

        /// <summary>
        /// 从object生成完整的Target
        /// </summary>
        public static void PostProcessing()
        {
            Dictionary<string, Target> name_target_pair = new Dictionary<string, Target>();

            foreach (var t in objects)
            {
                if (name_target_pair.ContainsKey(t.Item2.Name))
                {
                    throw new System.Exception
                        ($"Target defined! {t.Item2.Name}");
                }
                else
                {
                    name_target_pair.Add(t.Item2.Name, t.Item2);
                }
            }

            //生成依赖关系
            List<Target> targets = new List<Target>();
            foreach (var t in objects)
            {
                Target target = new Target
                {
                    Codes = t.Item2.Codes,
                    Name = t.Item2.Name
                };
                List<Target> deps = new List<Target>();

                foreach (var dep in t.Item1)
                {
                    if (name_target_pair.TryGetValue(dep, out Target value))
                    {
                        deps.Add(value);
                    }
                    else throw new System.Exception("Unknown target!");
                }

                if (deps.Count == 0)
                {
                    target.Deps = Array.Empty<Target>();
                }
                else
                {
                    target.Deps = deps.ToArray();
                }

                targets.Add(target);
            }

            GlobalContext.TargetList = targets.ToArray();
            return;
        }

        /// <summary>
        /// 入口函数
        /// </summary>
        public static void Run()
        {
            try
            {
                Preprocessor.Pretreatment(GlobalContext.Source, GlobalContext.FileName);
            }
            catch (Exception.PretreatmentException err)
            {
                Printer.Error("Pretreatment Error!");
                Printer.Error(err.ToString());
                throw;
            }

            try
            {
                Parse();
            }
            catch (Exception.GrammaticalException err)
            {
                Printer.Error("Grammatical Error!");
                Printer.Error(err.ToString());
                throw;
            }

            try
            {
                PostProcessing();
            }
            catch (System.Exception err)
            {
                Printer.Error("Post-processing Error!");
                Printer.Error(err.ToString());
                throw;
            }
        }

        //Distatic Area

        /// <summary>
        /// 局部变量
        /// </summary>
        public Dictionary<string, string> LocalVariables { get; init; }

        /// <summary>
        /// 程序计数器
        /// </summary>
        public int ProgramCounter { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public Target Source { get; set; }

        public Interpreter()
        {
            LocalVariables = new Dictionary<string, string>();
            ProgramCounter = 0;
        }

        /// <summary>
        /// 仅仅能执行宏
        /// </summary>
        /// <param name="t"></param>
        public void RunTarget(Target t)
        {
            try
            {
                Source = t;
                for (; ProgramCounter < Source.Codes.Length; ProgramCounter++)
                {
                    //预处理后的源文件
                    //No include & using
                    //No empty line
                    Tool.StringIterator stringIterator =
                        new Tool.StringIterator(Source.Codes[ProgramCounter].Item1);

                    //宏调用
                    if (stringIterator.Current == '\\')
                    {
                        stringIterator.Next();
                        //\"MacroName"{"Macro Param"}
                        //读取宏名
                        string macroName = stringIterator.ReadNextString();
                        if (macroName == null)
                            throw new
                                Exception.GrammaticalException
                                ("Error macro name", Source.Codes[ProgramCounter].Item2);

                        string macroParam;
                        //{
                        if (stringIterator.CanRead() && stringIterator.Current == '{')
                        {
                            stringIterator.Next();
                            //读取参数
                            macroParam = stringIterator.ReadNextString();

                            if (macroParam == null)
                                throw new
                                Exception.GrammaticalException
                                ("Error macro param", Source.Codes[ProgramCounter].Item2);

                            //}
                            if (!(stringIterator.CanRead() && stringIterator.Current == '}'))
                                throw new
                                    Exception.GrammaticalException
                                    ("Miss token `}`", Source.Codes[ProgramCounter].Item2);

                            stringIterator.Next();
                            stringIterator.SkipSpace();
                        }
                        else throw new
                                Exception.GrammaticalException
                                ("Miss token `{`", Source.Codes[ProgramCounter].Item2);

                        //末尾有其他字符
                        if (stringIterator.CanRead())
                            throw new
                                Exception.GrammaticalException
                                ("Remaining characters after macro", Source.Codes[ProgramCounter].Item2);

                        //当场调用
                        //局部调用
                        var result = MacroManager.CallMacro(macroName, macroParam, this);

                        //执行错误
                        if (result == null || result == false)
                            throw new
                                Exception.GrammaticalException
                                ("Macro run return error", Source.Codes[ProgramCounter].Item2);

                        continue;
                    }
                    //只支持调用宏
                    else throw new
                            Exception.GrammaticalException
                            ("Unknown macro call", Source.Codes[ProgramCounter].Item2);
                }
            }
            catch (Exception.GrammaticalException err)
            {
                throw;
            }
            catch (System.Exception err)
            {
                if (ProgramCounter < Source.Codes.Length)
                    throw new
                        Exception.GrammaticalException
                        ("Unknown Exception!", Source.Codes[ProgramCounter].Item2, err);
                else
                    throw new
                        Exception.GrammaticalException
                        ("Unknown Exception!", new LineInfo() { FileName = "Unknown", Line = -1 }, err);
            }
        }
    }
}
