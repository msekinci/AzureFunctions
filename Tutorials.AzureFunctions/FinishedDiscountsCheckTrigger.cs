using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading.Tasks;
using Tutorials.Core.Entities;

public static class FinishedDiscountsCheckTrigger
{
    [FunctionName("FinishedDiscountsCheckTrigger")]
    public static async Task RunAsync(
        [TimerTrigger("* * * * *")] TimerInfo myTimer,
        ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var cosmosConnectionString = configuration["Values:CosmosDbConnStr"];
        var cosmosDatabaseName = "discounts";
        var cosmosContainerName = "tutorials";

        var cosmosClientOptions = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };

        var cosmosClient = new CosmosClient(cosmosConnectionString, cosmosClientOptions);
        var container = cosmosClient.GetContainer(cosmosDatabaseName, cosmosContainerName);

        await CheckAndDeactivateDiscountsAsync(container, log);
    }

    private static async Task DeleteOldDiscountsAsync(Container container, List<Discount> allData)
    {
        var deletedDiscounts = allData
            .Where(x =>
                !x.IsActive ||
                x.DiscountCreatedDate.AddDays(x.DiscountDay) < DateTime.UtcNow)
            .ToList();

        foreach (var deletedDiscount in deletedDiscounts)
        {
            await container.DeleteItemAsync<Discount>(deletedDiscount.id.ToString(), new PartitionKey(deletedDiscount.SellerCode));
        }
    }

    private static async Task CheckAndDeactivateDiscountsAsync(Container container, ILogger log)
    {
        try
        {
            var data = container.GetItemLinqQueryable<Discount>(true).ToList();

            await DeleteOldDiscountsAsync(container, data);
            
            var count = data
                .Count(x => x.IsActive &&
                            x.DiscountCreatedDate.AddDays(x.DiscountDay) > DateTime.UtcNow);
            
            log.LogInformation("Aktif indirimli ürün sayısı: {Count}", count);
        }
        catch (Exception e)
        {
            log.LogError(e, e.Message);
        }
    }
}