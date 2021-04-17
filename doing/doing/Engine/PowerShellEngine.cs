using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Doing.Engine
{
    /// <summary>
    /// PowerShell引擎
    /// </summary>
    public class PowerShellEngine
    {
        /// <summary>
        /// 如果为null
        /// 则PowerShell未初始化
        /// </summary>
        private PowerShell? power = null;

        /// <summary>
        /// 内部的Shell
        /// </summary>
        public PowerShell? Shell { get
            {
                return power;
            } 
        }

        /// <summary>
        /// 源文件基础行号
        /// </summary>
        private int baseLine = 0;

        /// <summary>
        /// 源文件文件名
        /// </summary>
        private string baseFile = "";

        private readonly PSDataCollection<PSObject> pwshOutput = new();

        private PowerShellEngine() { }

        
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// 
        /// <param name="script">脚本源文件</param>
        /// <param name="linesPos">脚本所在代码行</param>
        /// <param name="sourceFile">脚本文件名称</param>
        /// 
        /// <returns>执行成功返回true，否则false。</returns>
        public bool ExecuteScript(
            string script,int linesPos = 0,string sourceFile = "<Temporary>")
        {
            if (Shell == null)
                throw new RuntimeException("Null powershell!");

            // 设置行号和源文件名称
            baseLine = linesPos;
            baseFile = sourceFile;

            // 清除之前的输出和命令
            Shell.Commands.Clear();
            Shell.Streams.ClearStreams();
            pwshOutput.Clear();

            // 调用命令
            Shell.AddScript(script);

            Shell.Invoke(null, pwshOutput);

            if (Shell.HadErrors)
                return false;

            else return true;
        }

        /// <summary>
        /// 创建一个Poweshell
        /// </summary>
        /// 
        /// <param name="name">运行空间名称</param>
        /// 
        /// <returns>实例</returns>
        public static PowerShellEngine Create(string name = "Default")
        {
            PowerShellEngine power = new();

            Runspace runspace = RunspaceFactory.CreateRunspace(Cmdlet.Cmdlet.InitCmlet());

            runspace.Open();

            runspace.Name = name;

            power.power = PowerShell.Create(runspace);

            // 初始化
            // 将错误输出同时定向到output
            power.power.Streams.Error.DataAdded += (sender, args) =>
            {
                ErrorRecord err = ((PSDataCollection<ErrorRecord>)sender!)[args.Index];

                // 存在位置信息
                if (err.InvocationInfo.PositionMessage.Trim().Length != 0)
                {
                    Tool.Printer.ErrLine
                ($"{err.ToString().Replace("{", "{{")}\n" +
                $"*** Err:{err.InvocationInfo.Line.Trim().Replace("{", "{{")}\n" +
                 "***     {0}\n" +
                $"***\tat `{power.baseFile}` " +
                $"Lines {err.InvocationInfo.ScriptLineNumber + power.baseLine} " +
                $"Pos {err.InvocationInfo.OffsetInLine}",
                new string('^', err.InvocationInfo.Line.Trim().Length));
                }
                else
                {
                    Tool.Printer.ErrLine(err.ToString().Replace("{","{{"));
                }
            };

            power.power.Streams.Warning.DataAdded += (sender, args) =>
            {
                WarningRecord warning = ((PSDataCollection<WarningRecord>)sender!)[args.Index];

                // 存在位置信息
                if(warning.InvocationInfo.PositionMessage.Trim().Length != 0)
                {
                    Tool.Printer.WarnLine
                    ($"{warning.ToString().Replace("{", "{{")}\n" +
                    $"*** Err:{warning.InvocationInfo.Line.Trim().Replace("{", "{{")}\n" +
                     "***     {0}\n" +
                    $"***\tat `{power.baseFile}` " +
                    $"Lines {warning.InvocationInfo.ScriptLineNumber + power.baseLine} " +
                    $"Pos {warning.InvocationInfo.OffsetInLine}",
                    new string('^', warning.InvocationInfo.Line.Trim().Length));
                }
                else
                {
                    Tool.Printer.WarnLine(warning.ToString());
                }
            };

            power.power.Streams.Progress.DataAdded += (sender, args) =>
            {
                ProgressRecord progress = ((PSDataCollection<ProgressRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{progress.ToString().Replace("{", "{{")}");
            };

            power.power.Streams.Information.DataAdded += (sender, args) =>
            {
                InformationRecord information = ((PSDataCollection<InformationRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{information.ToString().Replace("{", "{{")}");
            };

            power.power.Streams.Verbose.DataAdded += (sender, args) =>
            {
                VerboseRecord verbose = ((PSDataCollection<VerboseRecord>)sender!)[args.Index];
                Tool.Printer.PutLine($"{verbose.ToString().Replace("{", "{{")}");
            };

            power.pwshOutput.DataAdded += (sender, args) =>
            {
                PSObject output = ((PSDataCollection<PSObject>)sender!)[args.Index];
                Tool.Printer.PutLine($"{output.ToString().Replace("{", "{{")}");
            };

            return power;
        }
    }
}
