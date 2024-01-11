using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Tutorials.Core.Entities;

namespace Tutorials.AzureFunctions;

public static class DiscountNotificationsTrigger
{
    [FunctionName("DiscountNotificationsTrigger")]
    public static Task RunAsync(
        [ServiceBusTrigger("discountsqueue", Connection = "ServiceBusConnStr")] Discount discount, 
        ILogger log)
    {
        log.LogInformation($"New discount mails are sending!");
        SendMail(discount, log);
        log.LogInformation($"Mail sent");
        return Task.CompletedTask;
    }

    private static void SendMail(Discount discount, ILogger log)
    {
        log.LogInformation("" +
                           "{DiscountProductCode} kodlu üründe " +
                           "{DiscountSellerCode} kodlu mağazada " +
                           "{S} tarihine kadar " +
                           "{DiscountOriginalPrice} TL\'lik ürün sadece " +
                           "{DiscountDiscountedPrice} TL!", 
            discount.ProductCode, 
            discount.SellerCode, 
            DateTime.UtcNow.AddDays(discount.DiscountDay).ToString("dd/MM/yyyy"), 
            discount.OriginalPrice, 
            discount.DiscountedPrice);
    }
}