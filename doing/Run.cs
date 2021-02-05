/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Run.cs
 * Content: Run Source Files
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
    /// 执行命令单位
    /// </summary>
    class Run
    {
        /// <summary>
        /// Run规则
        /// </summary>
        [JsonInclude]
        public string Rule = "";

        /// <summary>
        /// Run环境变量
        /// </summary>
        [JsonInclude]
        public Dictionary<string, string> With =
            new Dictionary<string, string>();
    }
}
