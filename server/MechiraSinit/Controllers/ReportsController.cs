using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechiraSinit.Services;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")] // אבטחה: רק מי שיש לו Role של Manager בטוקן יכול להיכנס
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
            try
            {
                var report = _giftService.GetWinnersReport();

                // בדיקה אם הדוח חזר ריק (למשל אם עוד לא בוצעו הגרלות)
                if (report == null || !report.Any())
                {
                    return Ok(new { Message = "טרם נקבעו זוכים במערכת", Data = report });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                // הגנה מפני קריסה והחזרת הודעה ידידותית
                return StatusCode(500, new { Message = "שגיאה בהפקת דוח זוכים", Error = ex.Message });
            }
        }

        [HttpGet("income")]
        public IActionResult GetIncome()
        {
            try
            {
                var report = _purchaseService.GetIncomeReport();
                var total = _purchaseService.GetTotalIncome();

                // ולידציה לוגית: אם יש פירוט הכנסות אבל הסכום הכולל הוא 0 (או להפך) - זו נורת אזהרה
                if (total < 0)
                {
                    return BadRequest(new { Message = "נתוני ההכנסות לא תקינים" });
                }

                return Ok(new
                {
                    Details = report,
                    GrandTotal = total,
                    GeneratedAt = DateTime.Now // הוספת חותמת זמן לדוח
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "שגיאה בחישוב הכנסות המכירה", Error = ex.Message });
            }
        }
    }
}