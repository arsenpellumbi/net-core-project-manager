namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models
{
    public class FileContent
    {
        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public string Filename { get; set; }

        public string Path { get; set; }

        public byte[] FileBytes { get; set; }
    }
}