/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Rule.cs
 * Content: Rule Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace doing
{
    class Rule
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonInclude]
        public string Name = "";

        /// <summary>
        /// 程序
        /// </summary>
        [JsonInclude]
        public string Proc = "";

        /// <summary>
        /// 命令参数
        /// </summary>
        [JsonInclude]
        public string Args = "";

        /// <summary>
        /// Rule依赖
        /// </summary>
        [JsonInclude]
        public string Depend = "";

        /// <summary>
        /// 构建信息
        /// </summary>
        [JsonInclude]
        public string Introduction = "";
    }
}
