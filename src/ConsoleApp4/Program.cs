using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilver.Compiler.Debugger
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var projectToBuild = @"C:\Projects\OpenSilver\src\Runtime\Runtime\Runtime.OpenSilver.csproj";

            var buildProcess = StartDotNetBuild(projectToBuild);

            VSDebugger.AttachTo(buildProcess.Id);
        }

        private static Process StartDotNetBuild(string projectPath)
        {
            return Process.Start(@"dotnet", $@"build {projectPath}");
        }
    }
}
