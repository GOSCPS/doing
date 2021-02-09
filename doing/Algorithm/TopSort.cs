/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    TopSort.cs
 * Content: TopSort Source Files
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doing.Algorithm
{
    public static class TopSort
    {
        /// <summary>
        /// 访问设置
        /// </summary>
        private static Dictionary<Build.Target, bool> accessed
            = new Dictionary<Build.Target, bool>();

        /// <summary>
        /// 有序结果
        /// </summary>
        private static List<Build.Target> targets = new List<Build.Target>();

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns>排序结果</returns>
        public static Build.Target[] Sort()
        {
            foreach(var t in Build.GlobalContext.AimTarget)
            {
                Access(t);
            }
            return targets.ToArray();
        }

        private static void Access(Build.Target t)
        {
            if(!accessed.TryGetValue(t,out bool isInSatck))
            {
                //置入栈中
                accessed.Add(t, true);

                foreach(var dep in t.Deps)
                {
                    Build.Target depTarget = null;
                    foreach(var dt in Build.GlobalContext.TargetList)
                    {
                        if(dep.Name == dt.Name)
                        {
                            depTarget = dt;
                            break;
                        }
                    }
                    if (depTarget == null)
                        throw new System.Exception();

                    Access(depTarget);
                }
                targets.Add(t);

                accessed[t] = false;
            }
            //在堆栈中
            //循环依赖
            else if (isInSatck)
            {
                throw new System.Exception($"Circular dependency found in {t.Name}!");
            }
        }

    }
}
