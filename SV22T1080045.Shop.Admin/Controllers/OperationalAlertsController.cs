using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    [Route("api/operational-alerts")]
    public class OperationalAlertsController : Controller
    {
        private readonly IOperationalAlertService _operationalAlertService;

        public OperationalAlertsController(IOperationalAlertService operationalAlertService)
        {
            _operationalAlertService = operationalAlertService;
        }

        [HttpGet("feed")]
        public IActionResult Feed([FromQuery] DateTime? since)
        {
            var feed = _operationalAlertService.GetFeed(since);
            return Json(new
            {
                serverTime = feed.ServerTime,
                alerts = feed.Alerts.Select(a => new
                {
                    a.Key,
                    a.Kind,
                    a.Title,
                    a.Message,
                    a.TargetAnchor,
                    occurredAt = a.OccurredAt
                })
            });
        }
    }
}
