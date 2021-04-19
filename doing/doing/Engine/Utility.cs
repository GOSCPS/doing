//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Utility.cs
// Content: Utility Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Engine
{
    public static class Utility
    {


        /// <summary>
        /// 检查名称是否符合命名规则
        /// </summary>
        /// 
        /// <param name="name">要检查的字符串</param>
        /// <param name="err">第一个错误的字符，如果字符串长度为0则为\0</param>
        /// 
        /// <returns>符合返回true，否则false</returns>
        public static bool CheckName(string name, out char err)
        {
            err = '\0';

            if (name.Length == 0)
                return false;

            // 禁止数字开头
            if (char.IsDigit(name[0]))
            {
                err = name[0];
                return false;
            }

            for (int c = 0; c < name.Length; c++)
            {
                // 允许数字字母和下划线
                if (!(char.IsLetterOrDigit(name[c])) || name[c] == '_')
                {
                    err = name[c];
                    return false;
                }
            }

            return true;
        }










    }
}
