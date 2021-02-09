/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    PretreatmentException.cs
 * Content: PretreatmentException Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Exception
{
    /// <summary>
    /// 预处理错误
    /// </summary>
    class PretreatmentException : System.Exception
    {
        public PretreatmentException(string message) : base(message) { return; }

        public PretreatmentException(string message, System.Exception innerException)
            : base(message, innerException) { return; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
