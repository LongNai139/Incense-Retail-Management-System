using System.Text.RegularExpressions;

namespace SV22T1080045.Shop.Abstractions
{
    public static class PhoneNumberHelper
    {
        public const string VietnameseMobilePattern = @"^\s*(?:0|\+?84)[\s.\-()]*[3-9](?:[\s.\-()]*\d){8}\s*$";

        public static string NormalizeVietnameseMobile(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "";

            var digits = Regex.Replace(phone.Trim(), @"\D", "");
            if (digits.StartsWith("0084") && digits.Length >= 13)
                digits = "0" + digits[4..];
            else if (digits.StartsWith("84") && digits.Length >= 11)
                digits = "0" + digits[2..];

            return digits;
        }

        public static bool IsVietnameseMobile(string? phone)
        {
            var normalized = NormalizeVietnameseMobile(phone);
            return Regex.IsMatch(normalized, @"^0[3-9]\d{8}$");
        }
    }
}
