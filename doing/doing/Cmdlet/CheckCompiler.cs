using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Diagnostics;

namespace Doing.Cmdlet
{
    /// <summary>
    /// 检查Clang是否安装
    /// </summary>
    [Cmdlet("Check", "Clang")]
    class CheckClang : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Clang";

        /// <summary>
        /// 不填写此参数则只检测clang是否存在
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public string? Version { get; set; } = null;

        protected override void ProcessRecord()
        {
            // 启动clang
            Process process = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "clang",
                    Arguments = "--version",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };

            try
            {
                process.Start();
                process.WaitForExit();
            }
            // 找不到文件
            catch (InvalidOperationException)
            {
                WriteObject(false);
                return;
            }

            // 指定了RequiredVersion参数
            // 检查版本号
            if (Version != null)
            {
                Version required = new(Version);

                // clang --version第一行即版本号
                // #clang version 11.0.1
                string? verLine = process.StandardOutput.ReadLine();

                if (verLine == null)
                    throw new RuntimeException("Couldn't read clang version!");

                else if (!verLine.StartsWith("clang version "))
                    throw new RuntimeException("Couldn't parse clang version format!");

                Version current = new(verLine["clang version ".Length..].Trim());

                // <0
                // 当前版本小于required
                if (current.CompareTo(required) < 0)
                    WriteObject(false);
            }
            // 未指定版本
            // clang存在即返回
            else 
                WriteObject(true);
        }
    }

}
