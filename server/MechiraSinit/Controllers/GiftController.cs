using MechiraSinit.Dto;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly ILogger<GiftController> _logger; // 1. משתנה לוגר

        // 2. הזרקת הלוגר בבנאי
        public GiftController(IGiftService giftService, ILogger<GiftController> logger)
        {
            _giftService = giftService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllGifts([FromQuery] string? search, [FromQuery] string? sort)
        {
            // לוגים ב-Get הם אופציונליים, שמתי בהערה כדי לא להפציץ את הקונסול
            // _logger.LogInformation("Fetching all gifts. Search: {Search}, Sort: {Sort}", search, sort);
            return Ok(_giftService.GetAllGifts(search, sort));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateGift([FromBody] GiftDto giftDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ניסיון ליצור מתנה עם נתונים לא תקינים: {@ModelErrors}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("מנהל יוצר מתנה חדשה: {GiftName}", giftDto.Name);

            var newId = _giftService.AddGift(giftDto);

            _logger.LogInformation("מתנה נוצרה בהצלחה. ID: {GiftId}", newId);
            return CreatedAtAction(nameof(GetAllGifts), new { id = newId }, new { Id = newId });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateGift(int id, [FromBody] GiftDto giftDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("מעדכן מתנה {GiftId}", id);

            var updated = _giftService.UpdateGift(id, giftDto);

            if (!updated)
            {
                _logger.LogWarning("ניסיון לעדכן מתנה שלא קיימת: {GiftId}", id);
                return NotFound(new { Message = "Gift not found" });
            }

            return Ok(new { Message = "Gift updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult DeleteGift(int id)
        {
            _logger.LogInformation("מנהל מנסה למחוק מתנה: {GiftId}", id);
            try
            {
                if (!_giftService.DeleteGift(id))
                {
                    _logger.LogWarning("מחיקה נכשלה - מתנה {GiftId} לא נמצאה", id);
                    return NotFound();
                }

                _logger.LogInformation("מתנה {GiftId} נמחקה בהצלחה", id);
                return Ok(new { Message = "נמחק בהצלחה" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה במחיקת מתנה {GiftId} - כנראה יש רוכשים", id);
                return BadRequest(new { Message = "לא ניתן למחוק מתנה עם רוכשים", Error = ex.Message });
            }
        }

        [HttpPost("raffle/{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult PerformRaffle(int id)
        {
            _logger.LogInformation("--- התחלת הגרלה למתנה {GiftId} ---", id);
            try
            {
                var winner = _giftService.RunRaffle(id);

                _logger.LogInformation("ההגרלה הסתיימה! הזוכה המאושר במתנה {GiftId} הוא: {WinnerName}", id, winner.Name);

                return Ok(new { Message = "יש לנו זוכה!", WinnerName = winner.Name, WinnerEmail = winner.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ההגרלה נכשלה עבור מתנה {GiftId}", id);
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("winners-report")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetWinnersReport()
        {
            _logger.LogInformation("מפיק דוח זוכים...");
            var report = _giftService.GetWinnersReport();
            return Ok(report);
        }
    }
}