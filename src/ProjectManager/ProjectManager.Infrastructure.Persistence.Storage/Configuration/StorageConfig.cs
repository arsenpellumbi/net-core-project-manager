namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration
{
    public class StorageConfig
    {
        public StorageType Type { get; set; }
        public string StorageUrl { get; set; }
        public string Container { get; set; }
        public string TrashContainer { get; set; }
        public AmazonFsxConfig AmazonFsxConfig { get; set; }
        public AmazonS3Config AmazonS3Config { get; set; }
    }
}