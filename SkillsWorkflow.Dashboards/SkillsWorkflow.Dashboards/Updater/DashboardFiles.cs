using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Updater
{
    internal class DashboardFiles
    {
        private readonly string _repositoryDir;

        public DashboardFiles(string repositoryDir)
        {
            _repositoryDir = repositoryDir;
        }

        public IEnumerable<string> GetFilesToUpdate()
        {
            var directoriesToUpdate = GetDirectoriesToUpdate();
            var allDashboardFiles = new List<string>();
            foreach (var directoryToUpdate in directoriesToUpdate)
            {
                var jsonFiles = Directory.GetFiles(directoryToUpdate, "*.json", SearchOption.AllDirectories);
                foreach (var file in jsonFiles)
                {
                    var content = File.ReadAllText(file);
                    var json = JsonConvert.DeserializeObject<dynamic>(content);
                    if (null != json?.Default?.Value && json.Default.Value)
                    {
                        allDashboardFiles.Add(file);
                    }
                }
            }
            return allDashboardFiles;
        }

        internal IEnumerable<string> GetFilesForManifest()
        {
            var directoriesToUpdate = GetDirectoriesToUpdate();
            var allDashboardFiles = new List<string>();
            foreach (var directoryToUpdate in directoriesToUpdate)
            {
                var jsonFiles = Directory.GetFiles(directoryToUpdate, "*.json", SearchOption.AllDirectories);
                allDashboardFiles.AddRange(jsonFiles);
            }
            return allDashboardFiles;
        }

        private IEnumerable<string> GetDirectoriesToUpdate()
        {
            var getDirectoriesToUpdate = new List<string>();

            var getAllDirectoriesFromRepo = Directory.GetDirectories(_repositoryDir, "*", SearchOption.TopDirectoryOnly);
            foreach (var directoryName in getAllDirectoriesFromRepo)
            {
                var excludeDirs = new List<string>(new[] { "git", ".vs", "OnPremises", "SkillsWorkflow.Dashboards", "Manifest" });
                if (excludeDirs.Exists(i => directoryName.Contains(i)))
                {
                    continue;
                }
                getDirectoriesToUpdate.Add(directoryName);
            }
            return getDirectoriesToUpdate;
        }

    }
}
