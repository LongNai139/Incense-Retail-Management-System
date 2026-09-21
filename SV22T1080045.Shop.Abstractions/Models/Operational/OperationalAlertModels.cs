namespace SV22T1080045.Shop.Abstractions.Models.Operational
{
    public class OperationalAlertFeed
    {
        public DateTime ServerTime { get; set; }
        public List<OperationalAlertItem> Alerts { get; set; } = new();
    }

    public class OperationalAlertItem
    {
        public string Key { get; set; } = "";
        public string Kind { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? TargetAnchor { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
