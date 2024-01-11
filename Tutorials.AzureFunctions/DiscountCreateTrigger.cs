using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tutorials.Core.Models;

namespace Tutorials.AzureFunctions;

public static class DiscountCreateTrigger
{
    [FunctionName("DiscountCreateTrigger")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/discounts")] DiscountModel data, 
        [CosmosDB(databaseName: "discounts", containerName: "tutorials", Connection = "CosmosDbConnStr")] IAsyncCollector<dynamic> discountDbCollector,
        [ServiceBus(queueOrTopicName:"discountsqueue", Connection = "ServiceBusConnStr")] IAsyncCollector<dynamic> serviceBusCollector,
        ILogger log)
    {
        try
        {
            log.LogInformation("C# HTTP trigger function processed a request");
            
            log.LogInformation("New discount is creating...");
            log.LogInformation("Data: {SerializeObject}", JsonConvert.SerializeObject(data));

            var entity = data.ConvertDiscount();
            await discountDbCollector.AddAsync(entity);
            log.LogInformation("New discount is saved...");

            log.LogInformation("New discount is sending to queue...");
            await serviceBusCollector.AddAsync(entity);
            log.LogInformation("New discount is sent to queue...");

            return new OkObjectResult(data);
        }
        catch (Exception e)
        {
            log.LogError(e, e.Message);
            return new InternalServerErrorResult();
        }
    }
}