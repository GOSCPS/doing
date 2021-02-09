/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Preprocessor.cs
 * Content: Preprocessor Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace doing.Build.Interpreter
{
    /// <summary>
    /// 预处理器
    /// </summary>
    public static class Preprocessor
    {
        /// <summary>
        /// 最终处理过的行
        /// </summary>
        public static List<ValueTuple<LineInfo, string>> AllLines { get; set; }

        /// <summary>
        /// 预处理
        /// 出产AllLines
        /// </summary>
        /// <param name="lines">文件内容</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static void Pretreatment(string[] lines, string fileName)
        {
            if (AllLines == null)
                AllLines = new List<(LineInfo, string)>();

            //处理include
            //替换注释
            for (int ptr = 0; ptr < lines.Length; ptr++)
            {
                Tool.StringIterator ThisLineStringIterator = new Tool.StringIterator(lines[ptr].Trim());

                //忽略 注释和空行
                if (ThisLineStringIterator.Source == null ||
                    ThisLineStringIterator.Source.Length == 0 ||
                    ThisLineStringIterator.Current == '#')
                {
                    continue;
                }
                else
                {
                    string keyword = ThisLineStringIterator.ReadNextWord();

                    //include
                    if (keyword == "include")
                    {
                        //include "FileName"
                        ThisLineStringIterator.SkipSpace();
                        keyword = ThisLineStringIterator.ReadNextString();

                        ThisLineStringIterator.SkipSpace();
                        if (ThisLineStringIterator.CanRead())
                        {
                            throw new Exception.PretreatmentException($"Doing Error:include file " +
                                $"but Remaining characters in {fileName} at {ptr + 1}");
                        }
                        if (keyword == null)
                        {
                            throw new Exception.PretreatmentException
                                ($"Doing Error:include no file in {fileName} at {ptr + 1}");
                        }
                        else
                        {
                            //include file
                            try
                            {
                                string[] ls = File.ReadAllLines(keyword, Encoding.UTF8);
                                Pretreatment(ls, keyword);
                            }
                            catch (FileNotFoundException err)
                            {
                                throw new
                                    Exception.PretreatmentException
                                    ("Doing Error:include file not found", err);
                            }
                            continue;
                        }
                    }
                    //using
                    else if (keyword == "using")
                    {
                        //using "expand"
                        //expand位于doing.exe同级的expand目录下
                        ThisLineStringIterator.SkipSpace();
                        keyword = ThisLineStringIterator.ReadNextString();

                        ThisLineStringIterator.SkipSpace();
                        if (ThisLineStringIterator.CanRead())
                        {
                            throw new Exception.PretreatmentException($"Doing Error:using expand " +
                                $"but Remaining characters in {fileName} at {ptr + 1}");
                        }
                        if (keyword == null)
                        {
                            throw new Exception.PretreatmentException
                                ($"Doing Error:using no expand in {fileName} at {ptr + 1}");
                        }
                        else
                        {
                            //载入插件
                            if (!File.Exists(
                                 AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                 + "expand/" + keyword))
                                throw new
                                    Exception.PretreatmentException
                                    ("Doing Error:using expand not found");

                            Expand.ExpandManager.LoadExpandFromFile(
                                AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                 + "expand/" + keyword);
                            continue;
                        }
                    }
                }
                //非include和注释
                //填充行信息
                AllLines.Add((new LineInfo()
                {
                    FileName = fileName,
                    Line = ptr + 1
                }, lines[ptr].Trim()));
            }
            return;
        }

    }
}
