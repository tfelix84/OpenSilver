
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



using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml;

namespace OpenSilver.Compiler
{
    public class Updates : Microsoft.Build.Utilities.Task
    {
        private const string CompileAction = "compile";
        public string ProductVersion { get; set; }

        public override bool Execute()
        {
            Update("OpenSilver");
            return true;
        }

        internal async System.Threading.Tasks.Task Update(string productId)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                string identifier = GetIdentifier();
                string productVersion = GetProductVersion();

                //https://localhost:7234/api/Updates?id=%7BF3F4CE9F-FED3-4131-820A-656C543AE030%7D&productId=OpenSilver&version=1.1&editionId=None&action=compile

                UriBuilder uriBuilder = new UriBuilder("https://opensilver-service.azurewebsites.net/api/Updates");
                //UriBuilder uriBuilder = new UriBuilder("https://localhost:7234/api/Updates");

                uriBuilder.Query = $"id={identifier}&productId={productId}&version={productVersion}";
                //uriBuilder.Query = $"id={identifier}&productId={productId}&version={version}&editionId={editionId}&action={CompileAction}";
                string apiUrl = uriBuilder.ToString();

                //HttpContent content = new StringContent("");

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl); // In theory, we don't even need response since we don't care whether the call worked or not, but it can be useful when debugging.
            }
            catch (Exception ex)
            {
                // fail silently
            }
            finally
            {
                httpClient.Dispose();
            }

        }

        static string GetIdentifier()
        {
            string identifier = Properties.Settings.Default.Identifier;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                identifier = Guid.NewGuid().ToString();
                Properties.Settings.Default.Identifier = identifier;
                Properties.Settings.Default.Save();
            }

            return identifier;
        }

        string GetProductVersion()
        {
            if(string.IsNullOrEmpty(ProductVersion))
            {
                string compilerPath = PathsHelper.GetPathOfAssembly(Assembly.GetExecutingAssembly());
                string nuspecFilePath = Path.Combine(Path.GetDirectoryName(compilerPath), "../opensilver.nuspec");
                string nuspecFileContent = File.ReadAllText(nuspecFilePath);
                int versionStart = nuspecFileContent.IndexOf("<version>");
                int versionEnd = nuspecFileContent.IndexOf("</version>");
                int versionLength = "<version>".Length;
                if(versionStart != -1 && versionEnd != -1)
                {
                    ProductVersion = nuspecFileContent.Substring(versionStart + versionLength, versionEnd - versionStart - versionLength);
                }
            }
            return ProductVersion;
        }
    }
}
