//===========================================================
// 这个文件来自 GOSCPS(https://github.com/GOSCPS)
// 使用 GOSCPS 许可证
// File:    IncrementalCompilation.cs
// Content: IncrementalCompilation Source File
// Copyright (c) 2020-2021 GOSCPS 保留所有权利.
//===========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Cmdlet
{
    public class SourceFile
    {
        public FileInfo Source { get; init; }

        public SourceFile(FileInfo file)
        {
            Source = file;
        }
    }

    public class OutputFile
    {
        public FileInfo Output { get; init; }

        public OutputFile(FileInfo file)
        {
            Output = file;
        }
    }

    #region Add-Source
    /// <summary>
    /// 添加源文件
    /// </summary>
    [Cmdlet(VerbsCommon.Add,"Source")]
    [OutputType(typeof(SourceFile))]
    class AddSource : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Add-Source";

        /// <summary>
        /// 输入对象
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public PSObject? InputObject { get; set; } = null;

        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = false)]
        public PSObject? UserInput { get; set; } = null;

        /// <summary>
        /// 处理输入
        /// </summary>
        private void ProcessInput(object input)
        {
            Type inputType = input.GetType();

            //  处理数组
            if (inputType == typeof(object[]))
            {
                foreach (var obj in (object[])input)
                {
                    ProcessInput(obj);
                }
            }
            // 原样输出
            else if (inputType == typeof(SourceFile) || inputType == typeof(OutputFile))
            {
                WriteObject(input);
            }
            // 构造
            else if (inputType == typeof(string))
            {
                WriteObject(new SourceFile(new FileInfo((string)input)));
            }
            // 构造
            else if (inputType == typeof(FileInfo))
            {
                WriteObject(new SourceFile((FileInfo)input));
            }
            // 继续
            else if (inputType == typeof(PSObject))
            {
                ProcessInput(((PSObject)input).BaseObject);
            }
            // 抛出异常
            else
            {
                WriteError(Tool.ErrorHelper.NewError("The type of input object is unknown!", ErrorCategory.InvalidArgument, input));
                return;
            }
        }

        protected override void ProcessRecord()
        {
            if (InputObject == null)
            {
                WriteError(Tool.ErrorHelper.NewError("The input object is null!", ErrorCategory.InvalidArgument, InputObject));
                return;
            }
            ProcessInput(InputObject.BaseObject);

            if (UserInput != null)
            {
                ProcessInput(UserInput.BaseObject);
            }
        }
    }
    #endregion

    #region Add-Output
    /// <summary>
    /// 添加输出文件
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "Output")]
    [OutputType(typeof(SourceFile))]
    class AddOutput : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Add-Output";

        /// <summary>
        /// 输入对象
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public PSObject? InputObject { get; set; } = null;

        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = false)]
        public PSObject? UserInput { get; set; } = null;

        /// <summary>
        /// 处理输入
        /// </summary>
        private void ProcessInput(object input)
        {
            Type inputType = input.GetType();

            //  处理数组
            if (inputType == typeof(object[]))
            {
                foreach (var obj in (object[])input)
                {
                    ProcessInput(obj);
                }
            }
            // 原样输出
            else if (inputType == typeof(SourceFile) || inputType == typeof(OutputFile))
            {
                WriteObject(input);
            }
            // 构造
            else if (inputType == typeof(string))
            {
                WriteObject(new OutputFile(new FileInfo((string)input)));
            }
            // 构造
            else if (inputType == typeof(FileInfo))
            {
                WriteObject(new OutputFile((FileInfo)input));
            }
            // 继续
            else if(inputType == typeof(PSObject))
            {
                ProcessInput(((PSObject)input).BaseObject);
            }
            // 抛出异常
            else
            {
                WriteError(Tool.ErrorHelper.NewError("The type of input object is unknown!", ErrorCategory.InvalidArgument, input));
                return;
            }
        }

        protected override void ProcessRecord()
        {
            if (InputObject == null)
            {
                WriteError(Tool.ErrorHelper.NewError("The input object is null!", ErrorCategory.InvalidArgument, InputObject));
                return;
            }
            ProcessInput(InputObject.BaseObject);

            if(UserInput != null)
            {
                ProcessInput(UserInput.BaseObject);
            }
        }
    }

    #endregion

    #region Check-Compile
    [Cmdlet("Check","Compile")]
    class CheckCompile : System.Management.Automation.Cmdlet
    {
        public const string CallName = "Check-Compile";

        [Parameter(Position = 0,ValueFromPipeline = true, Mandatory = true)]
        public PSObject? InputObject { get; set; } = null;

        /// <summary>
        /// 处理输入
        /// </summary>
        private (SourceFile[],OutputFile[]) ProcessInput(object input)
        {
            List<SourceFile> sources = new();
            List<OutputFile> outputs = new();

            Type inputType = input.GetType();

            //  处理数组
            if (inputType == typeof(object[]))
            {
                foreach (var obj in (object[])input)
                {
                    var result = ProcessInput(obj);
                    sources.AddRange(result.Item1);
                    outputs.AddRange(result.Item2);
                }
            }
            // 原样输出
            else if (inputType == typeof(SourceFile))
            {
                sources.Add((SourceFile)input);
            }
            else if (inputType == typeof(OutputFile))
            {
                outputs.Add((OutputFile)input);
            }
            // 继续
            else if(inputType == typeof(PSObject))
            {
                var result = ProcessInput(((PSObject)input).BaseObject);
                sources.AddRange(result.Item1);
                outputs.AddRange(result.Item2);
            }
            else
            {
                WriteError(Tool.ErrorHelper.NewError("The type of input object is unknown!", ErrorCategory.InvalidArgument, input));
            }

            return (sources.ToArray(), outputs.ToArray());
        }

        protected override void ProcessRecord()
        {
            if (InputObject == null)
            {
                WriteError(Tool.ErrorHelper.NewError("The input object is null!", ErrorCategory.InvalidArgument, InputObject));
                return;
            }
            var compile = ProcessInput(InputObject.BaseObject);

            DateTime? oldestOutput = null;
            DateTime? newestSource = null;

            if(compile.Item1.Length == 0)
            {
                WriteError(Tool.ErrorHelper.NewError("No input sources!", ErrorCategory.InvalidArgument, compile.Item1));
                return;
            }
            else if(compile.Item2.Length == 0)
            {
                WriteError(Tool.ErrorHelper.NewError("no input outputs!", ErrorCategory.InvalidArgument, compile.Item2));
                return;
            }

            foreach(var source in compile.Item1)
            {
                // 源文件不存在!
                if (!source.Source.Exists)
                {
                    WriteError(Tool.ErrorHelper.NewError("The source isn't exists!", ErrorCategory.InvalidArgument, source));
                    return;
                }

                // 取最新的源文件修改时间
                if(newestSource == null || source.Source.LastWriteTime > newestSource)
                {
                    newestSource = source.Source.LastWriteTime;
                }
            }

            foreach (var output in compile.Item2)
            {
                // 输出文件不存在
                // 直接增量编译
                if (!output.Output.Exists)
                {
                    WriteObject(true);
                }

                // 取早的输出文件修改时间
                if (oldestOutput == null || output.Output.LastWriteTime < newestSource)
                {
                    oldestOutput = output.Output.LastWriteTime;
                }
            }

            // 最新的源文件修改时间 晚于 最早的输出文件的修改时间
            // 启动增量编译
            if(newestSource >= oldestOutput)
            {
                WriteObject(true);
            }
            else
            {
                WriteObject(false);
            }
        }

    }

    #endregion


}
