using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IManagementDAL _managementDAL;

        public ManagementService(IManagementDAL managementDAL)
        {
            _managementDAL = managementDAL;
        }

        public VoucherSaveResult CreateVoucher(Voucher voucher)
        {
            voucher.Code = voucher.Code?.Trim().ToUpperInvariant() ?? "";
            if (string.IsNullOrWhiteSpace(voucher.Code))
                return new VoucherSaveResult { Message = "Vui long nhap ma giam gia." };

            if (voucher.DiscountType == VoucherType.Percent && voucher.DiscountValue > 100)
                return new VoucherSaveResult { Message = "Giam theo phan tram khong duoc vuot qua 100%." };

            if (_managementDAL.VoucherCodeExists(voucher.Code, voucher.Id))
                return new VoucherSaveResult { Message = "Ma giam gia nay da ton tai." };

            voucher.Description = string.IsNullOrWhiteSpace(voucher.Description) ? voucher.Code : voucher.Description.Trim();
            voucher.CreatedTime = DateTime.Now;
            voucher.IsDeleted = false;
            voucher.UsedCount = 0;

            voucher.Id = _managementDAL.AddVoucher(voucher);

            return new VoucherSaveResult
            {
                Success = voucher.Id > 0,
                Message = voucher.Id > 0 ? $"Da tao ma giam gia {voucher.Code}." : "Khong the tao ma giam gia.",
                Voucher = voucher
            };
        }

        public VoucherToggleResult ToggleVoucher(int id)
        {
            var voucher = _managementDAL.GetVoucher(id);
            if (voucher == null)
                return new VoucherToggleResult { Message = "Khong tim thay ma giam gia." };

            voucher.IsActive = !voucher.IsActive;
            if (!_managementDAL.SetVoucherActive(voucher.Id, voucher.IsActive))
                return new VoucherToggleResult { Message = "Khong cap nhat duoc ma giam gia." };

            return new VoucherToggleResult
            {
                Success = true,
                Voucher = voucher,
                Message = voucher.IsActive
                    ? $"Da bat ma giam gia {voucher.Code}."
                    : $"Da tat ma giam gia {voucher.Code}."
            };
        }

        public List<Voucher> ListVouchers(int take = 10) => _managementDAL.ListVouchers(take);

        public List<ManagementOrderData> ListRecentOrders(int take = 10) => _managementDAL.ListRecentOrders(take);

        public List<CustomerManagementData> ListCustomers() => _managementDAL.ListCustomers();

        public List<CustomerPurchaseHistoryData> ListCustomerHistory(int customerId, int take) =>
            _managementDAL.ListCustomerHistory(customerId, take);

        public ManagementOrderDetailsData? GetOrderDetails(int orderId) => _managementDAL.GetOrderDetails(orderId);
    }
}
