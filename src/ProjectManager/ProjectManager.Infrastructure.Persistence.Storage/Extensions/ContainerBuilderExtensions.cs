using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Services;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterStorageService(this ContainerBuilder builder, StorageConfig storageConfig)
        {
            builder.RegisterInstance(storageConfig).AsSelf().SingleInstance();
            if (storageConfig.Type == StorageType.AmazonS3Storage)
            {
                builder
                    .Register(x => new AmazonS3StorageService(storageConfig))
                    .As<IStorageService>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder
                    .Register(x => new AmazonFsxStorageService(storageConfig))
                    .As<IStorageService>()
                    .InstancePerLifetimeScope();

                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                    storageConfig.AmazonFsxConfig.SharedFolderNetworkPath, storageConfig.Container));
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                    storageConfig.AmazonFsxConfig.SharedFolderNetworkPath, storageConfig.TrashContainer));
            }
        }
    }
}