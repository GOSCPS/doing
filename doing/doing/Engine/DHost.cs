//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    DHost.cs
// Content: DHost Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation.Host;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Engine
{
    /// <summary>
    /// Doing自己实现的Host
    /// </summary>
    public class DHost : PSHost
    {
        private int exitCode_ = 0;

        /// <summary>
        /// 退出代码
        /// </summary>
        public int ExitCode
        {
            get => exitCode_;
            private set => exitCode_ = value;
        }

        /// <summary>
        /// Host运行空间
        /// </summary>
        public Runspace HostRunspace { get; init; }

        public DHost(Runspace runspace)
        {
            HostRunspace = runspace;
        }

        public override CultureInfo CurrentCulture => System.Threading.Thread.CurrentThread.CurrentCulture;

        public override CultureInfo CurrentUICulture => System.Threading.Thread.CurrentThread.CurrentCulture;

        public override Guid InstanceId { get; } = Guid.NewGuid();

        public override string Name => "Doing Host";

        public override PSHostUserInterface UI => null!;

        public override Version Version => Program.DoingVersion;

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
            return;
        }

        public override void NotifyEndApplication()
        {
            return;
        }

        public override void SetShouldExit(int exitCode)
        {
            ExitCode = exitCode;
            return;
        }
    }
}
