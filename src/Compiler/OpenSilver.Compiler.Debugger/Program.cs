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
            var projectToBuild = @"C:\Projects\OpenSilver\src\Runtime\Controls.Data.Input\Controls.Data.Input.OpenSilver.csproj";

            var buildProcess = StartDotNetBuild(projectToBuild);

            VSDebugger.AttachTo(buildProcess.Id);
        }

        private static Process StartDotNetBuild(string projectPath)
        {
            return Process.Start(@"dotnet", $@"""C:\Program Files\dotnet\sdk\6.0.100-preview.6.21355.2\MSBuild.dll"" {projectPath} /t:Rebuild /p:Configuration=SL /clp:ErrorsOnly /bl");
            //return Process.Start(@"dotnet", $@"build {projectPath} --configuration SL");
        }
    }
}
