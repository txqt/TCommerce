using T.Library.Model.Discounts;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class DiscountDataSeed : SingletonBase<DiscountDataSeed>
    {
        public List<Discount> GetAll()
        {
            return new List<Discount>()
            {
                new Discount()
                {
                    Name = "Discount 5%",
                    DiscountType = DiscountType.AssignedToSkus,
                    UsePercentage = true,
                    DiscountPercentage = 5,
                    MaximumDiscountAmount = 20000,
                    StartDateUtc = DateTime.Now,
                    EndDateUtc = DateTime.Now.AddDays(10),
                    RequiresCouponCode = true,
                    CouponCode = "DISCOUNT5",
                    IsCumulative = false,
                    DiscountLimitation = DiscountLimitationType.NTimesPerCustomer,
                    LimitationTimes = 2,
                    MaximumDiscountedQuantity = 2,
                    IsActive = true,

                },
                new Discount()
                {
                    Name = "Discount 7%",
                    DiscountType = DiscountType.AssignedToSkus,
                    UsePercentage = true,
                    DiscountPercentage = 7,
                    MaximumDiscountAmount = 30000,
                    StartDateUtc = DateTime.Now,
                    EndDateUtc = DateTime.Now.AddDays(10),
                    RequiresCouponCode = true,
                    CouponCode = "DISCOUNT7",
                    IsCumulative = false,
                    DiscountLimitation = DiscountLimitationType.NTimesPerCustomer,
                    LimitationTimes = 2,
                    MaximumDiscountedQuantity = 2,
                    IsActive = true,
                },
                new Discount()
                {
                    Name = "Discount 10%",
                    DiscountType = DiscountType.AssignedToSkus,
                    UsePercentage = true,
                    DiscountPercentage = 10,
                    MaximumDiscountAmount = 50000,
                    StartDateUtc = DateTime.Now,
                    EndDateUtc = DateTime.Now.AddDays(10),
                    RequiresCouponCode = true,
                    CouponCode = "DISCOUNT10",
                    IsCumulative = false,
                    DiscountLimitation = DiscountLimitationType.NTimesPerCustomer,
                    LimitationTimes = 1,
                    MaximumDiscountedQuantity = 1,
                    IsActive = true,
                },
            };
        }
    }
}
