using MechiraSinit.Dto;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Manager")]
public class DonorsController : ControllerBase
{
    private readonly IDonorService _donorService;
    private readonly ILogger<DonorsController> _logger; // 1. משתנה לוגר

    // 2. הזרקה בבנאי
    public DonorsController(IDonorService donorService, ILogger<DonorsController> logger)
    {
        _donorService = donorService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search)
    {
        // לוג אינפורמטיבי (אופציונלי, לפעמים זה יוצר עומס אם יש המון קריאות)
        // _logger.LogInformation("שליפת כל התורמים. חיפוש: {Search}", search);
        return Ok(_donorService.GetAllDonors(search));
    }

    [HttpPost]
    public IActionResult Add([FromBody] DonorDto newDonor)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ניסיון ליצור תורם עם נתונים לא תקינים: {@Errors}", ModelState);
            return BadRequest(ModelState);
        }

        _logger.LogInformation("מנהל מוסיף תורם חדש: {Name}, {Phone}", newDonor.Name, newDonor.Phone);

        try
        {
            var newId = _donorService.AddDonor(newDonor);

            _logger.LogInformation("תורם נוצר בהצלחה. ID: {Id}", newId);
            return Ok(new { Message = "Donor created successfully", Id = newId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "שגיאה ביצירת תורם {Name}", newDonor.Name);
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] DonorDto donorDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (id != donorDto.Id)
        {
            _logger.LogWarning("אי התאמה ב-ID בעדכון תורם: URL={UrlId}, Body={BodyId}", id, donorDto.Id);
            return BadRequest("ID mismatch");
        }

        _logger.LogInformation("מעדכן פרטי תורם: {Id}", id);

        var success = _donorService.UpdateDonor(donorDto);
        if (!success)
        {
            _logger.LogWarning("ניסיון לעדכן תורם שלא נמצא: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("תורם {Id} עודכן בהצלחה", id);
        return Ok(new { Message = "Updated" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("מנהל מנסה למחוק תורם: {Id}", id);
        try
        {
            var success = _donorService.DeleteDonor(id);
            if (!success)
            {
                _logger.LogWarning("מחיקה נכשלה - תורם {Id} לא נמצא", id);
                return NotFound(new { Message = "התורם לא נמצא" });
            }

            _logger.LogInformation("תורם {Id} נמחק בהצלחה", id);
            return Ok(new { Message = "נמחק בהצלחה" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "שגיאה במחיקת תורם {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }
}