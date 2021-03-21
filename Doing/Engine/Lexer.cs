/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Lexer.cs
 * Content: Lexer Source File
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


namespace Doing.Engine
{
    /// <summary>
    /// Token类型
    /// </summary>
    enum TokenType
    {
        // 空Token
        null_token,

        // 标识符
        // 数字
        // 字符串
        identifier,
        number,
        str,

        // $ 
        dollar,

        // =
        equal,

        // != 
        not_equal,

        // < >
        smaller,
        bigger,

        // <= >=
        smaller_equal,
        bigger_equal,

        // ; .
        semicolon,
        dot,

        // ( )
        parentheses,
        parentheses_end,

        // { }
        curlyBraces,
        curlyBraces_end,

        // + - * /
        add,
        sub,
        mul,
        div,

        // !
        not,

        // ==
        double_equal,

        // ,
        comma,

        // :
        colon,

        /* *** 关键字 *** */
        keyword_target,

        keyword_else,

        keyword_if,

        keyword_global
    }

    /// <summary>
    /// 标记
    /// </summary>
    class Token
    {
        public TokenType type = TokenType.null_token;

        public long Line = 0;
        public string SourceFileName = "Unknown";

        private string value_str = "";
        private long value_num = 0;

        public string ValueString
        {
            get
            {
                if (type == TokenType.identifier ||
                    type == TokenType.str)
                    return value_str;

                else
                    throw new TypeException("Access string but type isn't identifier or string");
            }
            set
            {
                if (type == TokenType.identifier ||
                    type == TokenType.str)
                    value_str = value;

                else
                    throw new TypeException("Access string but type isn't identifier or string");
            }
        }

        public long ValueNumber
        {
            get
            {
                if (type == TokenType.number)
                    return value_num;

                else
                    throw new TypeException("Access number but type isn't number");
            }
            set
            {
                if (type == TokenType.number)
                    value_num = value;

                else
                    throw new TypeException("Access number but type isn't number");
            }
        }
    }


    /// <summary>
    /// 词法解析器
    /// </summary>
    static class Lexer
    {
        // 小端序!
        private static readonly UTF32Encoding encoding = new UTF32Encoding(false, false, true);


        public static Token[] Process(
            (string source, long row, string fileName)[] source)
        {
            List<Token> tokens = new List<Token>();

            // 合并字符串
            StringBuilder builder = new StringBuilder();

            foreach (var s in source)
                builder.Append($"{s.source}\n");

            string output = builder.ToString();

            // 处理
            int ptr = 0;
            long rowed = 0;

            try
            {
                // 循环
                while (ptr != output.Length)
                {
                    Token token = new Token();

                    if (output[ptr] == '\n')
                    {
                        rowed++;
                        ptr++;
                        continue;
                    }

                    else if (char.IsWhiteSpace(output[ptr]))
                    {
                        ptr++;
                        continue;
                    }

                    // 各种符号
                    else if (output[ptr] == '$')
                        token.type = TokenType.dollar;

                    else if (output[ptr] == '+')
                        token.type = TokenType.add;

                    else if (output[ptr] == '-')
                        token.type = TokenType.sub;

                    else if (output[ptr] == '*')
                        token.type = TokenType.mul;

                    else if (output[ptr] == '/')
                        token.type = TokenType.div;

                    else if (output[ptr] == '(')
                        token.type = TokenType.parentheses;

                    else if (output[ptr] == ')')
                        token.type = TokenType.parentheses_end;

                    else if (output[ptr] == '{')
                        token.type = TokenType.curlyBraces;

                    else if (output[ptr] == '}')
                        token.type = TokenType.curlyBraces_end;

                    else if (output[ptr] == '.')
                        token.type = TokenType.dot;

                    else if (output[ptr] == ',')
                        token.type = TokenType.comma;

                    else if (output[ptr] == ';')
                        token.type = TokenType.semicolon;

                    else if (output[ptr] == ':')
                        token.type = TokenType.colon;

                    // 需要后望的符号
                    // != & =
                    else if (output[ptr] == '!')
                    {

                        if ((ptr + 1) != output.Length && output[ptr + 1] == '=')
                        {
                            ptr++;
                            token.type = TokenType.not_equal;
                        }

                        else token.type = TokenType.not;
                    }

                    // > & >=
                    else if (output[ptr] == '>')
                    {

                        if ((ptr + 1) != output.Length && output[ptr + 1] == '=')
                        {
                            ptr++;
                            token.type = TokenType.bigger_equal;
                        }

                        else token.type = TokenType.bigger;
                    }

                    // < & <=
                    else if (output[ptr] == '<')
                    {

                        if ((ptr + 1) != output.Length && output[ptr + 1] == '=')
                        {
                            ptr++;
                            token.type = TokenType.smaller_equal;
                        }

                        else token.type = TokenType.smaller;
                    }

                    // = & ==
                    else if (output[ptr] == '=')
                    {

                        if ((ptr + 1) != output.Length && output[ptr + 1] == '=')
                        {
                            ptr++;
                            token.type = TokenType.double_equal;
                        }

                        else token.type = TokenType.equal;
                    }

                    // 数字
                    else if (char.IsDigit(output[ptr]))
                    {
                        string num = output[ptr].ToString();

                        while ((ptr + 1) != output.Length && char.IsDigit(output[ptr + 1]))
                        {
                            ptr++;
                            num += output[ptr].ToString();
                        }

                        token.type = TokenType.number;
                        token.ValueNumber = long.Parse(num);
                    }

                    // 标识符
                    // 仅以字母或下划线开头
                    else if (char.IsLetter(output[ptr]) || output[ptr] == '_')
                    {
                        string ident = output[ptr].ToString();

                        // 内容可为:字母，数字，下划线
                        while ((ptr + 1) != output.Length
                            && (
                            char.IsLetterOrDigit(output[ptr + 1])
                            || output[ptr + 1] == '_'))
                        {
                            ptr++;
                            ident += output[ptr].ToString();
                        }

                        // 检查关键字
                        if (ident == "target")
                        {
                            token.type = TokenType.keyword_target;
                        }
                        else if(ident == "else")
                        {
                            token.type = TokenType.keyword_else;
                        }
                        else if(ident == "if")
                        {
                            token.type = TokenType.keyword_if;
                        }
                        else if(ident == "global")
                        {
                            token.type = TokenType.keyword_global;
                        }
                        else
                        {
                            token.type = TokenType.identifier;
                            token.ValueString = ident;
                        }
                    }

                    // 字符串
                    else if (output[ptr] == '"')
                    {
                        StringBuilder str = new StringBuilder();

                        ptr++;

                        while (true)
                        {
                            if (ptr == output.Length)
                                throw new AnalyzeException("String open but get EOF!", source[rowed].row, source[rowed].fileName);

                            else if (char.IsControl(output[ptr]))
                                throw new AnalyzeException("String open but get Control!", source[rowed].row, source[rowed].fileName);

                            else if (output[ptr] == '"')
                                break;

                            // 转义
                            else if (output[ptr] == '\\')
                            {
                                ptr++;

                                if (ptr == output.Length)
                                    throw new AnalyzeException("Escape but get EOF!", source[rowed].row, source[rowed].fileName);

                                switch (output[ptr])
                                {
                                    case 't':
                                        str.Append('\t');
                                        break;

                                    case 'n':
                                        str.Append('\n');
                                        break;

                                    case '\\':
                                        str.Append('\\');
                                        break;

                                    case '"':
                                        str.Append('"');
                                        break;

                                    case '\'':
                                        str.Append('\'');
                                        break;

                                    case 'u':
                                        if ((ptr + 6) >= output.Length)
                                            throw new AnalyzeException("Unicode escape format error!",
                                                source[rowed].row, source[rowed].fileName);

                                        string unicode = output[(ptr + 1)..(ptr + 6)];
                                        long unicode_bin = Convert.ToInt64(unicode, 16);

                                        str.Append(encoding.GetString(BitConverter.GetBytes(unicode_bin)));

                                        ptr += 5;

                                        break;

                                    default:
                                        throw new AnalyzeException("Unknown escape type!", source[rowed].row, source[rowed].fileName);
                                }
                            }

                            else
                            {
                                str.Append(output[ptr]);
                            }

                            ptr++;
                        }

                        token.type = TokenType.str;
                        token.ValueString = str.ToString();
                    }

                    // 未知
                    else
                    {
                        throw new AnalyzeException("Unknown token!", source[rowed].row, source[rowed].fileName);
                    }

                    ptr++;
                    token.Line = source[rowed].row;
                    token.SourceFileName = source[rowed].fileName;
                    tokens.Add(token);
                }
            }
            catch (Exception)
            {
                if(rowed < source.Length)
                    Tool.Printer.Err($"Error at file {source[rowed].fileName} lines {source[rowed].row}");
                else
                    Tool.Printer.Err($"Error at file UNKNOWN lines {source[rowed].row}");

                throw;
            }

            return tokens.ToArray();
        }
    }
}
