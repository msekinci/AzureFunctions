using Tutorials.Core.Entities;

namespace Tutorials.Core.Models;

public class DiscountModel
{
    public string SellerCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public double OriginalPrice { get; set; }
    public double DiscountedPrice { get; set; }
    public int DiscountDay { get; set; }

    public Discount ConvertDiscount()
    {
        return new Discount()
        {
            SellerCode = SellerCode,
            ProductCode = ProductCode,
            DiscountDay = DiscountDay,
            DiscountedPrice = DiscountedPrice,
            OriginalPrice = OriginalPrice,
        };
    }
}