using MechiraSinit.Dto;
using MechiraSinit.Models;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public DonorsController(IDonorService donorService)
        {
            _donorService = donorService;
        }
        
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? search) // לוקח מה-URL
        {
            var donors = _donorService.GetAllDonors(search);
            return Ok(donors);
        }
        [HttpPost]
        public IActionResult Add([FromBody] DonorDto newDonor)
        {
            var newId = _donorService.AddDonor(newDonor);
            return Ok(new { Message = "Donor created successfully", Id = newId });
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] DonorDto donorDto)
        {
            // --- מצלמה 1: מה הגיע מהאנגולר? ---
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"[DEBUG] Update Request Received for ID: {id}");
            Console.WriteLine($"[DEBUG] Data in DTO -> ID: {donorDto.Id}, Name: '{donorDto.Name}', Email: '{donorDto.Email}'");

            if (id != donorDto.Id)
            {
                Console.WriteLine("[ERROR] ID Mismatch!");
                return BadRequest("ID mismatch");
            }

            var success = _donorService.UpdateDonor(donorDto);

            if (!success)
            {
                Console.WriteLine("[ERROR] Service returned False (Donor not found?)");
                return NotFound();
            }

            Console.WriteLine("[SUCCESS] Controller finished successfully.");
            Console.WriteLine("------------------------------------------");
            return Ok(new { Message = "Updated" });
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var success = _donorService.DeleteDonor(id);

                if (!success)
                {
                    return NotFound(new { Message = "התורם לא נמצא במערכת" });
                }

                return Ok(new { Message = "נמחק בהצלחה" });
            }
            catch (Exception ex)
            {
                // כאן אנחנו תופסים את ההודעה "אין אפשרות למחוק..." מהסרוויס
                // ומחזירים אותה למשתמש כשגיאה 400 (Bad Request)
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

