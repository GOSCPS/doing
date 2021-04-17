//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    Target.cs
// Content: Target Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    /// <summary>
    /// 获取用户设置的Target
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DoingUserTarget")]
    class GetDoingUserTarget : PSCmdlet
    {
        public const string CallName = "Get-DoingUserTarget";

        protected override void ProcessRecord()
        {
            WriteObject(Program.AimTargets.ToArray().Clone());
        }
    }

    /// <summary>
    /// 获取包含依赖的所有构建的Target
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DoingTotalTarget")]
    class GetDoingTotalTarget : PSCmdlet
    {
        public const string CallName = "Get-DoingTotalTarget";

        protected override void ProcessRecord()
        {
            WriteObject(Engine.Runner.TotalBuildTargets.Clone());
        }
    }

    /// <summary>
    /// 获取包含依赖的所有构建的Target
    /// </summary>
    [Cmdlet("Check", "DoingBuiltTarget")]
    class CheckDoingBuiltTarget : PSCmdlet
    {
        public const string CallName = "Check-DoingBuiltTarget";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true)]
        public string Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            WriteObject(Engine.Runner.CheckTargetFinish(Name));
        }
    }

    /// <summary>
    /// 获取包含依赖的所有构建的Target
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "DoingTarget")]
    class AddDoingTarget : PSCmdlet
    {
        public const string CallName = "Add-DoingTarget";

        /// <summary>
        /// 变量名
        /// 必选
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true)]
        public string Name { get; set; } = "";

        protected override void ProcessRecord()
        {
            if (Engine.Runner.TargetList.TryGetValue(Name, out (Engine.BuildModuleInfo, Engine.Target) value))
            {
                // 添加Main
                foreach(var mod in Engine.Runner.Modules)
                {
                    if(mod.SourceFile.FileName == value.Item2.FileName)
                    {
                        // 不为null
                        // 则modules拥有Main且尚未执行
                        // 则执行
                        if(mod.MainTarget != null)
                        {
                            Engine.Runner.AimTargets.Enqueue(mod.MainTarget);
                        }
                        break;
                    }
                }

                // 添加Target依赖及其依赖
                foreach(var dep in
                    Algorithm.Topological.Sort(new Engine.Target[] { value.Item2 }, Engine.Runner.AllExistTargets.ToArray()))
                {
                    Engine.Runner.AimTargets.Enqueue(dep);
                }
            }
            else throw new Exception.RuntimeException($"The target `{Name}` not found!");
        }
    }


}
