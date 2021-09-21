using DotNetForHtml5.Compiler;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    public abstract class TaskBase : Task
    {
        protected ILogger Logger { get; }

        public TaskBase()
        {
             Logger = new LoggerThatUsesTaskOutput(this);
        }
    }
}
