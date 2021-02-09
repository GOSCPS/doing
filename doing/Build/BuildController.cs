/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    BuildController.cs
 * Content: BuildController Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace doing.Build
{
    /// <summary>
    /// 构建控制器
    /// </summary>
    public static class BuildController
    {

        /// <summary>
        /// 根据已有GlobalContext构建
        /// </summary>
        public static void Build()
        {
            //完善目标
            foreach(var str in GlobalContext.AimTargetStrs)
            {
                bool got = false;
                foreach(var tar in GlobalContext.TargetList)
                {
                    if (tar.Name == str)
                    {
                        got = true;
                        GlobalContext.AimTarget.Add(tar);
                    }
                }
                if (!got)
                {
                    throw new System.Exception("Not found aim targets");
                }
            }
            //排序
            var sortedAimTargets = Algorithm.TopSort.Sort();

            foreach(var a in sortedAimTargets)
            {
                Printer.Common(a.Name);
            }


        }







    }
}
