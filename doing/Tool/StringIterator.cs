/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    StringIterator.cs
 * Content: StringIterator Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Text;

namespace doing.Tool
{
    public class StringIterator
    {

        //Unicode(UTF32)解码器
        //小端，启用安全检查
        //支持BOM
        private readonly UTF32Encoding encoding =
            new UTF32Encoding(false, true, true);

        /// <summary>
        /// 源文
        /// </summary>
        private readonly string source;

        public string Source { get { return source; } }

        /// <summary>
        /// 指针
        /// </summary>
        private int ptr_ = 0;

        public int Index { get { return ptr_; } }

        /// <summary>
        /// 当前的字符
        /// </summary>
        public char Current
        {
            get
            {
                return source[ptr_];
            }
        }

        public StringIterator(string source)
        {
            this.source = (string)source.Clone();
            return;
        }

        /// <summary>
        /// 跳过空格。
        /// ---Hello-World
        /// ↑--↑
        /// </summary>
        public void SkipSpace()
        {
            while (CanRead() && char.IsWhiteSpace(Current))
            {
                if (CanRead())
                    Next();
                else
                    break;
            }
            return;
        }

        /// <summary>
        /// 读取非空白单词直到空格
        /// ---Hello-World
        /// ---↑----↑
        /// return Hello
        /// </summary>
        /// <returns>
        /// 读取到的字符串
        /// </returns>
        public string ReadNextWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (CanRead() && (!char.IsWhiteSpace(Current)))
            {
                if (CanRead())
                {
                    stringBuilder.Append(Current);
                    Next();
                }
                else
                    break;
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 当前位置是否能读取
        /// Hello World
        /// -----------↑
        /// return false
        /// </summary>
        /// <returns></returns>
        public bool CanRead()
        {
            if (ptr_ < source.Length)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 下一个位置是否有效
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            if (ptr_ < (source.Length - 1))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 读取下几个字符
        /// Hello World
        /// ↑----------
        /// Read 3
        /// ---↑-------
        /// Return Hel
        /// </summary>
        /// <param name="range">要读取的字符数量</param>
        /// <returns>读取失败返回null</returns>
        public string ReadRange(uint range)
        {
            StringBuilder builder = new StringBuilder();
            uint org = range;
            while (CanRead() && range != 0)
            {
                builder.Append(Current);
                Next();
                range--;
            }
            if (builder.Length != org) return null;
            return builder.ToString();
        }

        /// <summary>
        /// 读取下一个字符串
        /// 包括转义
        /// "Hello World"
        /// ↑------------↑
        /// </summary>
        /// <returns>
        /// 返回null读取失败
        /// </returns>
        public string ReadNextString()
        {
            StringBuilder builder = new StringBuilder();

            if (Current != '"')
            {
                Printer.Error("Doing-StringIT Error:Miss token `\"`");
                return null;
            }

            Next();
            if (!CanRead()) return null;

            while (true)
            {
                //意外的末尾
                if (!CanRead())
                {
                    Printer.Error("Doing-StringIT Error:Miss token `\"`");
                    return null;
                }
                //结束
                if (Current == '"')
                {
                    Next();
                    break;
                }
                //转义
                else if (Current == '\\')
                {
                    if (!HasNext())
                    {
                        Printer.Error("Doing-StringIT Error:Escape but get EOF");
                        return null;
                    }
                    Next();
                    switch (Current)
                    {
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case '\\':
                            builder.Append('\\');
                            break;
                        case '"':
                            builder.Append('"');
                            break;
                        case '\'':
                            builder.Append('\'');
                            break;
                        case 'U':
                        case 'u':
                            Next();
                            string unicode = ReadRange(5);
                            if (unicode == null)
                            {
                                Printer.Error("Doing-StringIT Error:Escape unicode but get EOF");
                                return null;
                            }
                            byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(unicode, 16));
                            builder.Append(encoding.GetString(bytes));

                            // 已经抵达下一个字符
                            // 抵消末尾Next()
                            Back();
                            break;
                        default:
                            Printer.Error($"Doing-StringIT Error:Unknow Escape \\{Current}");
                            return null;
                    }
                }
                else
                {
                    builder.Append(Current);
                }

                Next();
            }

            return builder.ToString();
        }

        public void Next()
        {
            ptr_++;
        }

        public void Back()
        {
            ptr_--;
        }
    }
}