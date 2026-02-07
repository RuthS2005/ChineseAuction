using MechiraSinit.Dto;
using MechiraSinit.Models;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 1. חובה להיות מחובר (גם לקוח וגם מנהל)
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    // פונקציית עזר לשליפת ה-ID מהטוקן
    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (idClaim == null) return 0;
        return int.Parse(idClaim.Value);
    }

    [HttpPost("AddToCart")]
    public IActionResult AddToCart([FromBody] NewPurchaseDto dto)
    {
        // דורסים את ה-UserId שמגיע מהלקוח עם ה-ID האמיתי מהטוקן
        dto.UserId = GetCurrentUserId();

        try
        {
            _purchaseService.AddToCart(dto);
            return Ok(new { message = "הוסף לסל" });
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    // שימי לב: מחקתי את הפרמטר userId מהחתימה!
    //[HttpGet("my-cart")]
    //public IActionResult GetMyCart()
    //{
    //    int userId = GetCurrentUserId();
    //    var cart = _purchaseService.GetUserCart(userId);
    //    return Ok(cart);
    //}
    // GET: api/Purchases/Cart/5
    [HttpGet("Cart/{userId}")]
    public IActionResult GetCart(int userId)
    {
        try
        {
            // קריאה לסרוויס שיביא את הפריטים
            var cartItems = _purchaseService.GetUserCart(userId);

            // החזרת הנתונים (אפילו אם הסל ריק, זה יחזיר רשימה ריקה [])
            return Ok(cartItems);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    [HttpPost("checkout")]
    public IActionResult Checkout()
    {
        int userId = GetCurrentUserId(); // המזהה נלקח מהטוקן בלבד
        _purchaseService.Checkout(userId);
        return Ok(new { message = "שולם בהצלחה" });
    }
}