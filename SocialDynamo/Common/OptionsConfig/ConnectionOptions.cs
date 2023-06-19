namespace Common.OptionsConfig
{
    public  class ConnectionOptions
    {
        public string ServiceBus { get; set; } = string.Empty;
        public string AzureAccountDb { get; set; } = string.Empty;
        public string AzurePostsDb { get; set; } = string.Empty;
        public string AzureStorage { get; set; } = string.Empty;
    }
}
