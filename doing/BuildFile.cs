/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    BuildFile.cs
 * Content: BuildFile Source Files
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
    /// <summary>
    /// 构建文件
    /// </summary>
    class BuildFile
    {
        /// <summary>
        /// 要引用的文件
        /// </summary>
        [JsonInclude]
        public string[] Include = Array.Empty<string>();

        /// <summary>
        /// 要使用的内置设定
        /// </summary>
        [JsonInclude]
        public string Using = "";

        /// <summary>
        /// 依赖的版本
        /// </summary>
        [JsonInclude]
        public string Version = "";

        /// <summary>
        /// 构建目标
        /// </summary>
        [JsonInclude]
        public Target[] Targets = Array.Empty<Target>();

        /// <summary>
        /// 构建规则
        /// </summary>
        [JsonInclude]
        public Rule[] Rules = Array.Empty<Rule>();
    }
}
