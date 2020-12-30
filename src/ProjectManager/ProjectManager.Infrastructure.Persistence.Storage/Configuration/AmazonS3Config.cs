namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration
{
    public class AmazonS3Config
    {
        public string SystemName { get; set; }
        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string Bucket { get; set; }
        public int UrlValidTimeInHours { get; set; }
    }
}