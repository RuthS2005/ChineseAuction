using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechiraSinit.Services;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")] // רק למנהלים!
    public class ReportsController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly IPurchaseService _purchaseService;

        public ReportsController(IGiftService giftService, IPurchaseService purchaseService)
        {
            _giftService = giftService;
            _purchaseService = purchaseService;
        }

        [HttpGet("winners")]
        public IActionResult GetWinners()
        {
            var report = _giftService.GetWinnersReport();
            return Ok(report);
        }

        [HttpGet("income")]
        public IActionResult GetIncome()
        {
            var report = _purchaseService.GetIncomeReport();
            var total = _purchaseService.GetTotalIncome();

            return Ok(new
            {
                Details = report,
                GrandTotal = total
            });
        }
    }
}