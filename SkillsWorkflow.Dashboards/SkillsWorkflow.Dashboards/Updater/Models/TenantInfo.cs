namespace Updater.Models
{
    public class TenantInfo
    {
        //private TenantInfo tenantInfo;
        //public TenantInfo(TenantInfo tenantInfo)
        //{
        //    this.tenantInfo.Name = tenantInfo.Name;
        //    this.tenantInfo.Id = tenantInfo.Id;
        //    this.tenantInfo.AppId = tenantInfo.AppId;
        //    this.tenantInfo.AppSecret = tenantInfo.AppSecret;
        //}

        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public string AppId { get; internal set; }
        public string AppSecret { get; internal set; }
        public string ApiV2Url { get; internal set; }

        public override string ToString()
        {
            return $@"Name:{Name}" +
                $@"Id: {Id}" +
                $@"AppId: {AppId}" +
                $@"AppSecret: {AppSecret}" +
                $@"ApiV2Url: {ApiV2Url}";
        }
    }
}