namespace SV22T1080045.Shop.Models
{
    public static class OrderStatusLabels
    {
        public static string GetStatusText(int status) => status switch
        {
            1 => "Chờ xử lý",
            2 => "Đang chuẩn bị",
            3 => "Đang giao",
            4 => "Hoàn thành",
            -1 => "Đã hủy",
            _ => "Khác"
        };

        public static string GetPaymentStatusText(int paymentStatus) => paymentStatus switch
        {
            1 => "Đã thanh toán",
            2 => "Thanh toán thất bại",
            _ => "Chưa thanh toán"
        };

        public static string GetPaymentMethodText(int paymentMethod) => paymentMethod switch
        {
            2 => "VNPay",
            3 => "MoMo",
            4 => "Chuyển khoản QR",
            _ => "Thanh toán khi nhận hàng (COD)"
        };

        public static string FormatOrderCode(int orderId) => $"TH{orderId:D6}";

        /// <summary>Thứ tự bước hiển thị trên timeline (giống app sàn TMĐT).</summary>
        public static IReadOnlyList<(int StatusValue, string Label)> TimelineSteps { get; } =
        [
            (1, "Đơn đã đặt"),
            (1, "Chờ cửa hàng xác nhận"),
            (2, "Đang chuẩn bị hàng"),
            (3, "Đang giao hàng"),
            (4, "Giao thành công")
        ];

        public static string GetTimelineState(int orderStatus, int stepIndex)
        {
            if (stepIndex == 0)
                return "done";

            if (orderStatus == -1)
                return "cancelled";

            if (orderStatus >= 4)
                return "done";

            var requiredStatus = stepIndex;
            if (orderStatus > requiredStatus)
                return "done";
            if (orderStatus == requiredStatus)
                return "active";

            return "pending";
        }

        public static IReadOnlyList<OrderTimelineStepViewModel> BuildTimeline(int orderStatus)
        {
            var steps = TimelineSteps;
            var result = new List<OrderTimelineStepViewModel>(steps.Count);

            for (var i = 0; i < steps.Count; i++)
            {
                var label = steps[i].Label;
                if (orderStatus == -1 && i == steps.Count - 1)
                    label = "Đơn đã hủy";

                result.Add(new OrderTimelineStepViewModel
                {
                    Label = label,
                    State = GetTimelineState(orderStatus, i)
                });
            }

            return result;
        }
    }

    public class OrderTimelineStepViewModel
    {
        public string Label { get; set; } = "";
        public string State { get; set; } = "pending";
    }
}
