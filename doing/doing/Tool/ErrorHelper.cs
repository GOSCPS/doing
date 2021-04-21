//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    ErrorHelper.cs
// Content: ErrorHelper Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System.Management.Automation;

namespace Doing.Tool
{
    public static class ErrorHelper
    {

        public static ErrorRecord NewError(
            string msg, ErrorCategory type, object? errObj)
        {
            return new ErrorRecord(new
                DException.RuntimeException(msg),
                msg,
                type,
                errObj);
        }

    }
}
