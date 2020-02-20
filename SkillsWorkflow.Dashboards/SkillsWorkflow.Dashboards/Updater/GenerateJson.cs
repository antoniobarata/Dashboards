using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Updater
{
    internal class GenerateJson
    {
        private readonly IEnumerable<string> _dashboardfiles;

        public GenerateJson(IEnumerable<string> dashboardfiles)
        {
            _dashboardfiles = dashboardfiles;
        }

        public void ValidateJson(string templatesAggregatted)
        {
            JsonConvert.DeserializeObject(templatesAggregatted);
        }
        public string GenerateBatchJson()
        {
            var templatesAggregator = new StringBuilder();
            templatesAggregator.AppendLine(@"{");
            templatesAggregator.AppendLine(@"  ""DashboardDefinitionBatchAction"":""Import"",");
            templatesAggregator.AppendLine(@"  ""DashboardDefinitionImportModels"": [");

            foreach (var file in _dashboardfiles)
            {
                Console.WriteLine($@"Reading file {file}");
                var fileContent = File.ReadAllText(file, Encoding.UTF8);
                templatesAggregator.AppendLine(fileContent);
                templatesAggregator.Append(",");
            }
            templatesAggregator.Remove(templatesAggregator.Length - 2, 1);
            templatesAggregator.AppendLine(@"  ]");
            templatesAggregator.AppendLine(@"}");
            return templatesAggregator.ToString();
        }
    }
}
