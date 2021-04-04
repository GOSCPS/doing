/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    Topological.cs
 * Content: Topological Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Generic;


namespace Doing.Algorithm
{
    public static class Topological
    {

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Engine.Utility.Target[] Sort(
            Engine.Utility.Target[] source,
            Engine.Utility.Target[] total)
        {
            Dictionary<Engine.Utility.Target, bool> buf =
                new Dictionary<Engine.Utility.Target, bool>();

            Queue<Engine.Utility.Target> output = new Queue<Engine.Utility.Target>();


            // 挨个处理
            foreach (var visit in source)
            {
                Visit(total, visit, buf, output);
            }

            return output.ToArray();
        }

        private static void Visit(
            Engine.Utility.Target[] total,
            Engine.Utility.Target visiter,
            Dictionary<Engine.Utility.Target, bool> buf,
            Queue<Engine.Utility.Target> output)
        {

            // 检测target是否已经经过处理
            if (buf.TryGetValue(visiter, out bool isMade))
            {
                if (isMade)
                {
                    throw new Engine.RuntimeException($"Circular dependency detected. At target `{visiter.name}`.");
                }
                else return;
            }
            else
            {
                buf.Add(visiter, true);

                // 检查依赖
                foreach (var depStr in visiter.deps)
                {
                    Engine.Utility.Target? dep = null;

                    foreach (var ddepStr in total)
                    {
                        // 找到依赖
                        if (ddepStr.name == depStr)
                        {
                            dep = ddepStr;
                            break;
                        }
                    }

                    if (dep == null)
                        throw new Engine.RuntimeException($"Miss depend `{depStr}` in target `{visiter.name}` !");

                    // 检查依赖的依赖
                    Visit(total, dep, buf, output);
                }

                output.Enqueue(visiter);
                buf.Remove(visiter);
                buf.Add(visiter, false);
            }
        }
    }
}
