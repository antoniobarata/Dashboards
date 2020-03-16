using System.Threading.Tasks;
using Updater.Models;
using SkillsWorkflow.Services.Tenants.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using LibGit2Sharp;

namespace Updater
{
    class Program
    {
        private static Dictionary<string, string> argumentsDictionary;

        public static void SetupArgumentsList()
        {
            argumentsDictionary = new Dictionary<string, string>();
            argumentsDictionary.Add("-tenantApi", string.Empty);
            argumentsDictionary.Add("-tenantsApiKey", string.Empty);
            argumentsDictionary.Add("-repositoryDir", string.Empty);
            argumentsDictionary.Add("-region", string.Empty);
        }

        static async Task Main(string[] args)
        {
            SetupArgumentsList();
            foreach (var arg in args)
            {
                Console.WriteLine($@"Arg: {arg}");
            }
            for (int index = 0; args.Length > index; index = index + 2)
            {
                argumentsDictionary[args[index]] = args[index + 1];
            }

            if (argumentsDictionary.Any(a => string.IsNullOrWhiteSpace(a.Value)))
            {
                var invalidArgument = argumentsDictionary.FirstOrDefault(a => string.IsNullOrWhiteSpace(a.Value));
                Console.WriteLine($@"Error: Missing argument for {invalidArgument.Key}");
                Console.WriteLine($@"Usage");
                Console.WriteLine($@"updater -tenantApi <url> -tenantsApiKey <key> -repositoryDir <git repository root>");
                Environment.Exit(-1);
            }
         
            string tenantsApiUrl = argumentsDictionary["-tenantApi"];
            string tenantsApiKey = argumentsDictionary["-tenantsApiKey"];
            string repositoryDir = argumentsDictionary["-repositoryDir"];
            string region = argumentsDictionary["-region"];
            var files = new DashboardFiles(repositoryDir);
            //var filesForManifest = files.GetFilesForManifest();

            //var manifestAutomation = new Manisfest(new HttpClient(), null);
            //manifestAutomation.GenerateManifest(repositoryDir);

            await UpdateTenants(tenantsApiUrl, tenantsApiKey, files.GetFilesToUpdate(), region);
        }

        private static async Task UpdateTenants(string tenantsApiUrl, string tenantsApiKey, IEnumerable<string> dashboardfiles, string region)
        {
            var templatesAggregator = new GenerateJson(dashboardfiles);
            var templatesAggregatted = templatesAggregator.GenerateBatchJson();
            templatesAggregator.ValidateJson(templatesAggregatted);
            var client = new TenantServiceClient(new TenantServiceClientSettings { AccessKey = tenantsApiKey, BaseUrl = new System.Uri(tenantsApiUrl) }, new HttpClient());
            var tenants = await client.GetListAsync();
            foreach (var tenant in tenants)
            {
                if (tenant.Region.ToUpperInvariant() == region.ToUpperInvariant())
                    continue;
                Console.WriteLine($"{tenant.Name,-30} {tenant.Region,-15} {tenant.Id}");
                var application = tenant.Applications.FirstOrDefault();
                var binding = tenant.AuthorityBindings.FirstOrDefault(a => "APIV2" == a.Site.ToUpperInvariant());

                var tenantInfo = new TenantInfo
                {
                    Name = tenant.Name,
                    Id = tenant.Id.ToString(),
                    AppId = application.AppId.ToString(),
                    AppSecret = application.AppSecret,
                    ApiV2Url = binding.AuthorityBinding
                };

                var updater = new DashboardTenantUpdater(tenantInfo, new HttpClient(), templatesAggregatted);
                await updater.DoUpdateAsync();
            }
        }
    }

    internal class Manisfest
    {
        private HttpClient _httpClient;
        private readonly IEnumerable<string> _files;

        public Manisfest(HttpClient httpClient, IEnumerable<string> files)
        {
            _httpClient = httpClient;
            _files = files;
        }

        public void GenerateManifest(string repositoryPath )
        {
            //using (var repo = new Repository(repositoryPath))
            //{
            //    foreach (var entry in repo.Index)
            //    {
            //        Console.WriteLine("{0} {1} {2}       {3}",
            //            Convert.ToString((int)entry.Mode, 8),
            //            entry.Id.ToString(), (int)entry.StageLevel, entry.Path);
            //        entry.
            //    }

            //}

            //var manifestDictionary = new Dictionary<string, FileManifestModel>()
            //foreach (var file in _files)
            //{
            //    Console.WriteLine($@"Reading file {file}");
            //    var fileContent = File.ReadAllText(file, Encoding.UTF8);
            //    var jsonObject = JsonConvert.DeserializeObject<dynamic>(fileContent);
            //    manifestDictionary.Add(file, new FileManifestModel()
            //    {
            //        FileFullName = file,
            //        Default = jsonObject.Default,
            //        Name = jsonObject.Name,
            //        Id = jsonObject.Id,
            //        LastCommit = new Commit()
            //        {
            //            Changes = jsonObject.Commit
            //        }
            //    });
            //}

        }
    }
}
