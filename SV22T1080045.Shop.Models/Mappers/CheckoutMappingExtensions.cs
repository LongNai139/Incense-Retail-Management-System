using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.Requests.Checkout;
using SV22T1080045.Shop.Models.ViewModels.Checkout;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class CheckoutMappingExtensions
    {
        public static CheckoutSuccessViewModel ToCheckoutSuccessViewModel(
            this Order order,
            IEnumerable<OrderDetail> details)
        {
            return new CheckoutSuccessViewModel
            {
                OrderId = order.Id,
                ShippingName = order.ShippingName ?? "",
                ShippingPhone = order.ShippingPhone ?? "",
                ShippingAddress = order.ShippingAddress ?? "",
                TotalAmount = order.TotalAmount,
                VoucherCode = order.VoucherCode,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount - order.DiscountAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Details = details
                    .Select(d => new CheckoutOrderDetailViewModel
                    {
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    })
                    .ToList()
            };
        }

        public static Customer ToCustomer(this QuickRegisterRequest request, Order order)
        {
            return new Customer
            {
                CustomerName = string.IsNullOrWhiteSpace(request.CustomerName)
                    ? order.ShippingName ?? ""
                    : request.CustomerName,
                Phone = request.Phone,
                Password = request.Password,
                Role = "Customer",
                Address = order.ShippingAddress
            };
        }
    }
}
