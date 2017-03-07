using Microsoft.PowerBI.Api.V1;
using Microsoft.PowerBI.Api.V1.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReportingProvisioning
{
    class Program
    {
        static string sqlUserName = ConfigurationManager.AppSettings["sqlLogin"];
        static string sqlPassword = ConfigurationManager.AppSettings["sqlPassword"];

        static string apiEndpointUri = ConfigurationManager.AppSettings["PowerBIApiUri"];
        static string accessKey = ConfigurationManager.AppSettings["AzureWorkspaceCollectionKey"];
        static string workspaceName = ConfigurationManager.AppSettings["workspaceName"];

        static string workspaceCollectionName = ConfigurationManager.AppSettings["AzureWorkspaceCollectionName"];

        static Dictionary<string, string> reports = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            if (args.Length<1)
            {
                Console.WriteLine("Pass a configuration file as a parameter (see ReportConfiguration.txt)!");
                return;
            }

            var reader = File.OpenText(args[0]);
            string line = reader.ReadLine();

            while (line != null)
            {
                string filePath = line;
                string reportName = reader.ReadLine();
                reports.Add(reportName, filePath);
                line = reader.ReadLine();
            }

            AsyncPump.Run(async delegate
            {
                await Run();
            });
        }

        static async Task Run()
        {
            try
            {
                //check if the workspaces are available and try to create them if they are not available
                var workspaces = await GetWorkspaces(workspaceCollectionName);
                var workspace = (from a in workspaces where a.DisplayName.Equals(workspaceName) select a).FirstOrDefault();

                if (workspace == null)
                {
                    workspace = await CreateWorkspace(workspaceCollectionName, workspaceName);
                    Console.WriteLine("Workspace for internal reports is created");
                }
                else
                {
                    Console.WriteLine("Workspace for internal reports is found");
                }

                Console.WriteLine("Clear all datasets and reports");
                await DeleteAllDatasets(workspaceCollectionName, workspace.WorkspaceId);

                Console.WriteLine("Looking for reports");

                string[] files;

                if (reports.Count > 0)
                {
                    foreach (var rep in reports)
                    {
                        Console.WriteLine($"Importing {rep.Value}");
                        var import = await ImportPbix(workspaceCollectionName, workspace.WorkspaceId, rep.Key, rep.Value);

                        Console.WriteLine($"Updating connection string for {rep.Value}");
                        var dataSetID = (from a in import.Datasets where a.Name.Equals(rep.Key) select a.Id).FirstOrDefault();
                        await UpdateConnection(workspaceCollectionName, workspace.WorkspaceId, dataSetID, sqlUserName, sqlPassword);
                    }
                }

                Console.WriteLine("Update is completed");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ooops, something broke: {0}", ex);
                Console.WriteLine();
            }
        }

        static async Task<Workspace> CreateWorkspace(string workspaceCollectionName, string workspaceName)
        {
            using (var client = await CreateClient())
            {
                return await client.Workspaces.PostWorkspaceAsync(workspaceCollectionName, new CreateWorkspaceRequest(workspaceName));
            }
        }

        static async Task DeleteAllDatasets(string workspaceCollectionName, string workspaceId)
        {
            using (var client = await CreateClient())
            {
                ODataResponseListDataset response = await client.Datasets.GetDatasetsAsync(workspaceCollectionName, workspaceId);

                if (response.Value.Any())
                {
                    foreach (Dataset d in response.Value)
                    {
                        await client.Datasets.DeleteDatasetByIdAsync(workspaceCollectionName, workspaceId, d.Id);
                        Console.WriteLine("{0}:  {1}", d.Name, d.Id);
                    }
                }
                else
                {
                    Console.WriteLine("No Datasets found in this workspace");
                }
            }
        }

        static async Task<IEnumerable<Workspace>> GetWorkspaces(string workspaceCollectionName)
        {
            using (var client = await CreateClient())
            {
                var response = await client.Workspaces.GetWorkspacesByCollectionNameAsync(workspaceCollectionName);
                return response.Value;
            }
        }

        static async Task<Import> ImportPbix(string workspaceCollectionName, string workspaceId, string datasetName, string filePath)
        {
            using (var fileStream = File.OpenRead(filePath.Trim('"')))
            {
                using (var client = await CreateClient())
                {
                    // Set request timeout to support uploading large PBIX files
                    client.HttpClient.Timeout = TimeSpan.FromMinutes(60);
                    client.HttpClient.DefaultRequestHeaders.Add("ActivityId", Guid.NewGuid().ToString());

                    // Import PBIX file from the file stream
                    var import = await client.Imports.PostImportWithFileAsync(workspaceCollectionName, workspaceId, fileStream, datasetName);

                    // Example of polling the import to check when the import has succeeded.
                    while (import.ImportState != "Succeeded" && import.ImportState != "Failed")
                    {
                        import = await client.Imports.GetImportByIdAsync(workspaceCollectionName, workspaceId, import.Id);
                        Console.WriteLine("Checking import state... {0}", import.ImportState);
                        Thread.Sleep(1000);
                    }

                    return import;
                }
            }
        }

        static async Task ListDatasets(string workspaceCollectionName, string workspaceId)
        {
            using (var client = await CreateClient())
            {
                ODataResponseListDataset response = await client.Datasets.GetDatasetsAsync(workspaceCollectionName, workspaceId);

                if (response.Value.Any())
                {
                    foreach (Dataset d in response.Value)
                    {
                        Console.WriteLine("{0}:  {1}", d.Name, d.Id);
                    }
                }
                else
                {
                    Console.WriteLine("No Datasets found in this workspace");
                }
            }
        }

        static async Task UpdateConnection(string workspaceCollectionName, string workspaceId, string datasetId, string login, string password)
        {
            using (var client = await CreateClient())
            {
                var datasources = await client.Datasets.GetGatewayDatasourcesAsync(workspaceCollectionName, workspaceId, datasetId);

                // Reset your connection credentials
                var delta = new GatewayDatasource
                {
                    CredentialType = "Basic",
                    BasicCredentials = new BasicCredentials
                    {
                        Username = login,
                        Password = password
                    }
                };

                if (datasources.Value.Count != 1)
                {
                    Console.Write("Expected one datasource, updating the first");
                }

                // Update the datasource with the specified credentials
                await client.Gateways.PatchDatasourceAsync(workspaceCollectionName, workspaceId, datasources.Value[0].GatewayId, datasources.Value[0].Id, delta);
            }
        }

        static async Task<PowerBIClient> CreateClient()
        {
            if (accessKey == null)
            {
                Console.Write("Access Key: ");
                accessKey = Console.ReadLine();
                Console.WriteLine();
            }

            // Create a token credentials with "AppKey" type
            var credentials = new TokenCredentials(accessKey, "AppKey");

            // Instantiate your Power BI client passing in the required credentials
            var client = new PowerBIClient(credentials);

            // Override the api endpoint base URL.  Default value is https://api.powerbi.com
            client.BaseUri = new Uri(apiEndpointUri);

            return client;
        }

    }
}
