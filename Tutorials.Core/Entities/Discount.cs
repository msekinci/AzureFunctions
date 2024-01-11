using System.Text.Json.Serialization;

namespace Tutorials.Core.Entities;

public class Discount
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string SellerCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public double OriginalPrice { get; set; }
    public double DiscountedPrice { get; set; }
    public int DiscountDay { get; set; }
    public DateTime DiscountCreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}