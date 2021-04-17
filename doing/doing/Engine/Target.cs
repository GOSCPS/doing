using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Engine
{
    /// <summary>
    /// doing标准Target类
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Target名称
        /// </summary>
        public string Name { get; set; } = "#Unknown#";

        /// <summary>
        /// Target依赖
        /// </summary>
        public string[] Deps { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = "Unknown";

        /// <summary>
        /// 起始行号
        /// </summary>
        public int StartLine { get; set; } = 0;

        /// <summary>
        /// 源代码
        /// </summary>
        public string SourceCode { get; set; } = string.Empty;
    }
}
