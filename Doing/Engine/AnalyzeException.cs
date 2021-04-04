/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    AnalyzeException.cs
 * Content: AnalyzeException Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;


namespace Doing.Engine
{
    class AnalyzeException : Exception
    {
        public long Row { get; init; }
        public string FileName { get; init; }

        public AnalyzeException(string msg, long row = 0, string fileName = "Unknown") : base(msg)
        {
            Row = row;
            FileName = fileName;
        }

        public override string ToString()
        {
            return $"At file `{FileName}` line {Row}\n" + base.ToString();
        }


    }
}
