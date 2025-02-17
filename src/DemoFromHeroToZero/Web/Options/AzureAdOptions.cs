namespace Web.Options
{
    public class AzureAdOptions
    {
        public string Instance { get; set; } = "https://login.microsoftonline.com/";
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string CallbackPath { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string ClientSecret { get; set; }
        public string SubscriptionId { get; set; }
        public string AcrRG { get; set; }
        public string DockerHostUrl { get; set; }
    }
}