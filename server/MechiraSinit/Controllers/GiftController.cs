using MechiraSinit.Dto;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        // 1. משתנה שיחזיק את הסרוויס
        private readonly IGiftService _giftService;

        // 2. בנאי (Constructor) - כאן מתבצעת ההזרקה (DI)
        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet]
        public IActionResult GetAllGifts()
        {
            // פניה לסרוויס כדי להביא נתונים מה-SQL
            var gifts = _giftService.GetAllGifts();
            return Ok(gifts);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateGift([FromBody] GiftDto giftDto)
        {
            // פניה לסרוויס כדי לשמור
            var newId = _giftService.AddGift(giftDto);

            // מחזירים ללקוח את ה-ID החדש שנוצר
            return Ok(newId);
        }

        // Update endpoint
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateGift(int id, [FromBody] GiftDto giftDto)
        {
            try
            {
                var updated = _giftService.UpdateGift(id, giftDto);
                if (!updated)
                    return NotFound(new { Message = "Gift not found" });

                return Ok(new { Message = "Gift updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")] // חובה! רק מנהל מוחק
        public IActionResult DeleteGift(int id)
        {
            try
            {
                var success = _giftService.DeleteGift(id);
                if (!success)
                {
                    return NotFound(new { Message = "המתנה לא נמצאה" });
                }
                return Ok(new { Message = "המתנה נמחקה בהצלחה" });
            }
            catch (Exception ex)
            {
                // זה יקרה אם תנסי למחוק מתנה שכבר קנו לה כרטיסים (בגלל קשרי גומלין ב-SQL)
                return BadRequest(new { Message = "לא ניתן למחוק מתנה שנרכשו ממנה כרטיסים", Error = ex.Message });
            }
        }

        [HttpPost("raffle/{id}")]
        [Authorize(Roles = "Manager")]
        public IActionResult PerformRaffle(int id)
        {
            try
            {
                var winner = _giftService.RunRaffle(id);

                return Ok(new
                {
                    Message = "יש לנו זוכה!",
                    WinnerName = winner.Name,
                    winner.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}