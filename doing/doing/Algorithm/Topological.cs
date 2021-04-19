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
        public static Engine.Target[] Sort(
            Engine.Target[] source,
            Engine.Target[] total)
        {
            Dictionary<Engine.Target, bool> buf =
                new ();

            Queue<Engine.Target> output = new();


            // 挨个处理
            foreach (var visit in source)
            {
                Visit(total, visit, buf, output);
            }

            return output.ToArray();
        }

        private static void Visit(
            Engine.Target[] total,
            Engine.Target visiter,
            Dictionary<Engine.Target, bool> buf,
            Queue<Engine.Target> output)
        {

            // 检测target是否已经经过处理
            if (buf.TryGetValue(visiter, out bool isMade))
            {
                if (isMade)
                {
                    throw new DException.RuntimeException($"Circular dependency detected. At target `{visiter.Name}`.");
                }
                else return;
            }
            else
            {
                buf.Add(visiter, true);

                // 检查依赖
                foreach (var depStr in visiter.Deps)
                {
                    Engine.Target? dep = null;

                    foreach (var ddepStr in total)
                    {
                        // 找到依赖
                        if (ddepStr.Name == depStr)
                        {
                            dep = ddepStr;
                            break;
                        }
                    }

                    if (dep == null)
                        throw new DException.RuntimeException($"Miss depend `{depStr}` in target `{visiter.Name}` !");

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
