using MechiraSinit.Data;
using MechiraSinit.Dto;
using MechiraSinit.Models;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IGiftService _giftService;
        private readonly AppDbContext _context;
        private readonly ILogger<PurchasesController> _logger; // 1. משתנה לוגר

        // 2. הזרקת הלוגר בבנאי
        public PurchasesController(
            IPurchaseService purchaseService,
            IGiftService giftService,
            AppDbContext context,
            ILogger<PurchasesController> logger) // <--- הוספתי כאן
        {
            _purchaseService = purchaseService;
            _giftService = giftService;
            _context = context;
            _logger = logger; // <--- השמה
        }

        private int GetCurrentUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] NewPurchaseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var gift = _giftService.GetGiftById(dto.GiftId);
            if (gift == null) return NotFound("המתנה לא נמצאה");

            if (!string.IsNullOrEmpty(gift.WinnerName))
            {
                _logger.LogWarning("ניסיון לקנות כרטיסים למתנה שכבר הוגרלה: {GiftName}", gift.Name);
                return BadRequest(new { message = "מכירה סגורה: כבר בוצעה הגרלה למתנה זו" });
            }

            dto.UserId = GetCurrentUserId();

            try
            {
                _purchaseService.AddToCart(dto);
                // לוג אינפורמטיבי רגיל
                _logger.LogInformation("משתמש {UserId} הוסיף לסל כרטיסים למתנה {GiftId}", dto.UserId, dto.GiftId);
                return Ok(new { message = "הוסף לסל" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בהוספה לסל למשתמש {UserId}", dto.UserId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("Cart/{userId}")]
        public IActionResult GetCart(int userId)
        {
            int currentUserId = GetCurrentUserId();
            if (userId != currentUserId && !User.IsInRole("Manager"))
            {
                _logger.LogWarning("Access Denied: משתמש {CurrentId} ניסה לגשת לעגלה של {TargetId}", currentUserId, userId);
                return Forbid();
            }
            return Ok(_purchaseService.GetUserCart(userId));
        }

        [HttpPost("Checkout/{userId}")]
        public IActionResult Checkout(int userId)
        {
            _logger.LogInformation("--- התחלת תהליך תשלום (Checkout) למשתמש {UserId} ---", userId);

            try
            {
                var userCart = _context.Purchases
                    .Include(p => p.Gift)
                    .Where(p => p.UserId == userId && !p.IsPaid)
                    .ToList();

                if (!userCart.Any())
                {
                    _logger.LogWarning("ניסיון תשלום על עגלה ריקה למשתמש {UserId}", userId);
                    return BadRequest("העגלה ריקה");
                }

                foreach (var item in userCart)
                {
                    if (!string.IsNullOrEmpty(item.Gift.WinnerName))
                    {
                        _logger.LogWarning("תשלום נכשל: המתנה {GiftName} כבר הוגרלה בזמן שהייתה בעגלה", item.Gift.Name);
                        return BadRequest(new { message = $"המתנה '{item.Gift.Name}' כבר הוגרלה. הסר אותה מהסל כדי להמשיך." });
                    }

                    item.IsPaid = true;
                }

                _context.SaveChanges();

                _logger.LogInformation("✅ תשלום בוצע בהצלחה למשתמש {UserId}. מספר פריטים: {Count}", userId, userCart.Count);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ שגיאה קריטית בביצוע תשלום למשתמש {UserId}", userId);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}