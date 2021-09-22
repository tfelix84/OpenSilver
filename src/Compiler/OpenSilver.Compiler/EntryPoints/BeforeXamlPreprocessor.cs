

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

using DotNetForHtml5.Compiler.OtherHelpersAndHandlers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class BeforeXamlPreprocessor : TaskBase // AppDomainIsolatedTask
    {
        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public string Flags { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string ReferencesPaths { get; set; }
        public string SourceAssemblyForPass2 { get; set; }

        [Required]
        public bool IsBridgeBasedVersion { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string NameOfAssembliesThatDoNotContainUserCode { get; set; }

        public bool IsProcessingCSHTML5Itself { get; set; }

#if BRIDGE && !CSHTML5BLAZOR
        [Required]
#endif
        public string TypeForwardingAssemblyPath { get; set; }

#if CSHTML5BLAZOR
        // the assemblies are setted by the AssemblyReferenceValidator
        public Microsoft.Build.Framework.ITaskItem[] ResolvedReferences { get; set; }
#endif

        public override bool Execute()
        {
            string passNumber = (IsSecondPass ? "2" : "1");
            string operationName = string.Format("C#/XAML for HTML5: BeforeXamlPreprocessor (pass {0})", passNumber);
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    //------- DISPLAY THE PROGRESS -------
                    Logger.WriteMessage(operationName + " started.");

                    //-----------------------------------------------------
                    // Note: we create a static instance of the "ReflectionOnSeparateAppDomainHandler" to avoid reloading the assemblies for each XAML file.
                    // We dispose the static instance in the "AfterXamlPreprocessor" task.
                    //-----------------------------------------------------

                    if (IsSecondPass && string.IsNullOrEmpty(SourceAssemblyForPass2))
                        throw new Exception(operationName + " failed because the SourceAssembly parameter was not specified during the second pass.");

                    var reflectionHandler = ServiceProvider.GetService<ReflectionHandlerFactory>().GetOrCreate(TypeForwardingAssemblyPath);

#if BRIDGE
                    //todo: if we are compiling CSHTML5 itself (or CSHTML5.Stubs), we need to process the XAML files in CSHTML5,
                    // and for that we need to load the XAML types, so we need to load the previous version of CSHTML5 (from
                    // the NuGet package). Note: this is not supposed to lead to a circular reference because it is only used
                    // for the XamlPreprocessor to generate the .xaml.g.cs files from the .xaml files.
                    // To do so, we need to stop skipping the processing of the CSHTML5 and CSHTML5.Stubs assemblies (c.f.
                    // "Skip the assembly if it is not a user assembly" in "LoadAndProcessReferencedAssemblies").
#endif
                    // we load the source assembly early in case we are processing the CSHTML5.
                    if (IsSecondPass && IsProcessingCSHTML5Itself)
                    {
                        reflectionHandler.LoadAssembly(SourceAssemblyForPass2, loadReferencedAssembliesToo: true, isBridgeBasedVersion: IsBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: NameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }
#if CSHTML5BLAZOR
                    // work-around: reference path string is not correctly setted so we set it manually
                    string referencePathsString = OpenSilverHelper.ReferencePathsString(ResolvedReferences);
#endif
                    // Retrieve paths of referenced .dlls and load them:
                    HashSet<string> referencePaths = (referencePathsString != null) ? new HashSet<string>(referencePathsString.Split(';')) : new HashSet<string>();

                    referencePaths.RemoveWhere(s => !s.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || s.Contains("DotNetBrowser") || s.EndsWith(@"\bridge.dll", StringComparison.InvariantCultureIgnoreCase));

                    var coreAssembliesPaths = referencePaths.Where(AssembliesLoadHelper.IsCoreAssembly).ToArray();

                    // Note: we ensure that the Core assemblies are loaded first so that types such as "XmlnsDefinitionAttribute" are known when loading the other assemblies.
                    foreach (var coreAssemblyName in AssembliesLoadHelper.CoreAssembliesNames)
                    {
                        var coreAssemblyPath = coreAssembliesPaths.FirstOrDefault(path => coreAssemblyName.Equals(Path.GetFileNameWithoutExtension(path), StringComparison.InvariantCultureIgnoreCase));
                        if (coreAssemblyPath != null)
                        {
                            reflectionHandler.LoadAssembly(coreAssemblyPath, loadReferencedAssembliesToo: false, isBridgeBasedVersion: IsBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: NameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                            referencePaths.Remove(coreAssemblyPath);
                        }
                    }

                    foreach (string referencedAssembly in referencePaths)
                    {
                        reflectionHandler.LoadAssembly(referencedAssembly, loadReferencedAssembliesToo: false, isBridgeBasedVersion: IsBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: NameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }

                    // Load "mscorlib.dll" too (this is useful for resolving Mscorlib types in XAML, such as <system:String x:Key="TestString" xmlns:system="clr-namespace:System;assembly=mscorlib">Test</system:String>)
                    reflectionHandler.LoadAssemblyMscorlib(isBridgeBasedVersion: IsBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: NameOfAssembliesThatDoNotContainUserCode);

                    // Load for reflection the source assembly itself and the referenced assemblies if second path:
                    if (IsSecondPass && !IsProcessingCSHTML5Itself)
                    {
                        reflectionHandler.LoadAssembly(SourceAssemblyForPass2, loadReferencedAssembliesToo: true, isBridgeBasedVersion: IsBridgeBasedVersion, isCoreAssembly: false, nameOfAssembliesThatDoNotContainUserCode: NameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies: false);
                    }

                    bool isSuccess = true;

                    //------- DISPLAY THE PROGRESS -------
                    Logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed.") + "\". IsSecondPass: " + IsSecondPass.ToString() + ". Source assembly file: \"" + (SourceAssemblyForPass2 ?? "").ToString());

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ReflectionHandlerFactory>().Release();

                Logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
