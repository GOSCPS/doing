/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Target.cs
 * Content: Target Source Files
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
    /// 执行目标
    /// </summary>
    class Target
    {
        [JsonInclude]
        public string Name = "";

        //构建依赖Target
        [JsonInclude]
        public string[] Depend = Array.Empty<string>();

        [JsonInclude]
        public Run[] RunList = Array.Empty<Run>();
    }
}
