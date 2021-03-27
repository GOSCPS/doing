/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Variable.cs
 * Content: Variable Source File
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


namespace Doing.Engine.Utility
{
    /// <summary>
    /// 变量
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// 变量
        /// </summary>
        public enum VariableType
        {
            /// <summary>
            /// 没有类型
            /// </summary>
            NoType,

            /// <summary>
            /// 字符串
            /// </summary>
            String,

            /// <summary>
            /// 有符号数字
            /// </summary>
            Number,

            /// <summary>
            /// 布尔变量
            /// </summary>
            Boolean,

            /// <summary>
            /// C#对象
            /// </summary>
            Object
        }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string name = "#Temporary Value#";

        private VariableType type_ = VariableType.NoType;
        /// <summary>
        /// 变量类型
        /// </summary>
        public VariableType Type
        {
            get
            {
                return type_;
            }
            set
            {
                switch (value)
                {
                    case VariableType.NoType:
                        DynamicValue = null;
                        break;

                    case VariableType.Number:
                        DynamicValue = 0;
                        break;

                    case VariableType.String:
                        DynamicValue = "";
                        break;

                    case VariableType.Boolean:
                        DynamicValue = false;
                        break;

                    case VariableType.Object:
                        DynamicValue = null;
                        break;
                }

                type_ = value;
            }
        }

        private dynamic? DynamicValue = null;

        public string ValueString
        {
            get
            {
                if (Type != VariableType.String)
                    throw new CompileException($"Access {Type:G} as a string!");

                return DynamicValue!;
            }
            set
            {
                if (Type != VariableType.String)
                    throw new CompileException($"Access {Type:G} as a string!");

                DynamicValue = value;
            }
        }

        public long ValueNumber
        {
            get
            {
                if (Type != VariableType.Number)
                    throw new CompileException($"Access {Type:G} as a number!");

                return DynamicValue;
            }
            set
            {
                if (Type != VariableType.Number)
                    throw new CompileException($"Access {Type:G} as a number!");

                DynamicValue = value;
            }
        }

        public bool ValueBoolean
        {
            get
            {
                if (Type != VariableType.Boolean)
                    throw new CompileException($"Access {Type:G} as a boolean!");

                return DynamicValue;
            }
            set
            {
                if (Type != VariableType.Boolean)
                    throw new CompileException($"Access {Type:G} as a boolean!");

                DynamicValue = value;
            }
        }


        public object? ValueObject
        {
            get
            {
                if (Type != VariableType.Object)
                    throw new CompileException($"Access {Type:G} as a object!");

                return DynamicValue;
            }
            set
            {
                if (Type != VariableType.Object)
                    throw new CompileException($"Access {Type:G} as a object!");

                DynamicValue = value;
            }
        }
    }
}
