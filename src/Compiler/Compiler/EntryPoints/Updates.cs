
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



using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Net.Http;

namespace OpenSilver.Compiler
{
    public class Updates : Microsoft.Build.Utilities.Task
    {
        private const string CompileAction = "compile";
        public string ProductVersion { get; set; }
        [Required]
        public string PackagePath { get; set; }

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
                string date = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

                //https://localhost:7234/api/Updates?id=%7BF3F4CE9F-FED3-4131-820A-656C543AE030%7D&productId=OpenSilver&version=1.1&editionId=None&action=compile

                UriBuilder uriBuilder = new UriBuilder("https://opensilver-service.azurewebsites.net/api/Updates");
                //UriBuilder uriBuilder = new UriBuilder("https://localhost:7234/api/Updates");

                uriBuilder.Query = $"id={identifier}&productId={productId}&version={productVersion}&date={date}";
                //uriBuilder.Query = $"id={identifier}&productId={productId}&version={version}&editionId={editionId}&action={CompileAction}";
                string apiUrl = uriBuilder.ToString();


                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
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
            string id;
            id = OpenSilverSettings.Instance.GetValue(Constants.UPDATES_IDENTIFIER_KEY);
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
                OpenSilverSettings.Instance.SetValue(Constants.UPDATES_IDENTIFIER_KEY, id);
                OpenSilverSettings.Instance.SaveSettings();
            }
            return id;
        }

        string GetProductVersion()
        {
            if (string.IsNullOrEmpty(ProductVersion))
            {
                string pathToPackageRootDirectory = Path.GetDirectoryName(PackagePath);
                ProductVersion = PackagePath.Substring(pathToPackageRootDirectory.Length + 1);
            }
            return ProductVersion;
        }
    }
}
