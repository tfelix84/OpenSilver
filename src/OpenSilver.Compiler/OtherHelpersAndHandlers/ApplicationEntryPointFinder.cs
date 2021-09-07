

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



#if !BRIDGE && !CSHTML5BLAZOR
extern alias DotNetForHtml5Core;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using DotNetForHtml5.Compiler.Common;

namespace DotNetForHtml5.Compiler
{
    internal static class ApplicationEntryPointFinder
    {
        private static readonly AssemblyLoadContext context = new AssemblyLoadContext("Context", true);
        public static void GetFullNameOfClassThatInheritsFromApplication(string pathOfAssemblyThatContainsEntryPoint, out string applicationClassFullName, out string assemblyName, out string assemblyFullName)
        {
            var assembly = context.LoadFromAssemblyPath(pathOfAssemblyThatContainsEntryPoint);

            // Call the method that finds the type that inherits from Application:
            applicationClassFullName = FindApplicationClassFullName(assembly);

            // Get the assembly name and full name too:
            assemblyName = assembly.GetName().Name;
            assemblyFullName = assembly.FullName;

            // Unload the assembly (so that we can later delete it if necessary):
            context.Unload();
        }


        public static string FindApplicationClassFullName(Assembly assembly)
        {
#if BRIDGE || CSHTML5BLAZOR
            throw new NotSupportedException();
#else
                // Get the base "Application" type:
#if SILVERLIGHTCOMPATIBLEVERSION
                Type baseApplicationType = typeof(DotNetForHtml5Core::System.Windows.Application);
#else
                Type baseApplicationType = typeof(DotNetForHtml5Core::Windows.UI.Xaml.Application);
#endif

                // Find a class that inherits from the base application type:
                Type applicationType = null;
                foreach (Type type in _assembly.GetTypes())
                {
                    if (baseApplicationType.IsAssignableFrom(type))
                    {
                        applicationType = type;
                        break;
                    }
                }

                // If no such class is found, throw an exception:
                if (applicationType == null)
                    throw new Exception("Error: the project contains no entry point. The project must contain a class that inherits from Application.");

                return applicationType.FullName;
#endif
        }
    }
}
