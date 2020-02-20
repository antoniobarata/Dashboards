using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Updater.Models;

namespace Updater
{
    internal class DashboardTenantUpdater
    {
        private readonly HttpClient _client;
        private readonly string _batchJson;
        private readonly TenantInfo _tenantInfo;

        public DashboardTenantUpdater(TenantInfo tenantInfo, HttpClient client, string batchJson )
        {
            _tenantInfo = tenantInfo;
            _client = client;
            _batchJson = batchJson;
#if DEBUG
            _client.BaseAddress = new Uri($@"https://{_tenantInfo.ApiV2Url}");
#else
            _client.BaseAddress = new Uri($@"https://{_tenantInfo.ApiV2Url}");
#endif
        }

        private async Task ProcessBatchUpdate(string jsonContent)
        {
            SetDefaultRequestHeaders();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var stringTask = _client.PostAsync($@"{_client.BaseAddress}/api/v3/dashboard-definitions", content);
            
            var msg = await stringTask;
            Console.WriteLine("= RESULT ====");
            Console.WriteLine($@"Status code: {msg.StatusCode}");
            Console.WriteLine($@"Content: {await msg.Content.ReadAsStringAsync()}");
        }

        private void SetDefaultRequestHeaders()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-AppId", _tenantInfo.AppId);
            _client.DefaultRequestHeaders.Add("X-AppSecret", _tenantInfo.AppSecret);
            _client.DefaultRequestHeaders.Add("X-AppTenant", _tenantInfo.Id);
        }

        public async Task DoUpdateAsync()
        {            
            await ProcessBatchUpdate(_batchJson);
        }
    }
}
