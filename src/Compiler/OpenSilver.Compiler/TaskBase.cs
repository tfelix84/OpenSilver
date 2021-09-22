using DotNetForHtml5.Compiler;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    public abstract class TaskBase : Task
    {

        private static IServiceProvider Provider;

        protected ILogger Logger { get; }

        protected static IServiceProvider ServiceProvider
        {
            get
            {
                if (Provider is null)
                {
                    Provider = Bootstrapper.ServiceProvider;
                }

                return Provider;
            }
        }

        public TaskBase()
        {
             Logger = new LoggerThatUsesTaskOutput(this);
        }
    }

    public static class Bootstrapper
    {
        private static IServiceProvider Provider;
        private static IServiceProvider ConfigureServices() =>
            new ServiceCollection()
            .AddSingleton(new ReflectionHandlerFactory())
            .BuildServiceProvider();

        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (Provider is null)
                {
                    Provider = ConfigureServices();
                }

                return Provider;
            }
        }
    }
}
