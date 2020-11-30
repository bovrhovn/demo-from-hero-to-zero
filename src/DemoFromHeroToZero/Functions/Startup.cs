using System;
using Functions;
using Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var storageKey = Environment.GetEnvironmentVariable("StorageKey");

            builder.Services.AddScoped<IStorageWorker, AzureStorageWorker>(_ =>
                new AzureStorageWorker(storageKey,
                    Environment.GetEnvironmentVariable("ContainerName")));
        }
    }
}