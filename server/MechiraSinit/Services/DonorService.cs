using MechiraSinit.Data;
using MechiraSinit.Dto;
using MechiraSinit.Models;

namespace MechiraSinit.Services
{// ... existing using statements and namespace ...

    public class DonorService : IDonorService
    {
        private readonly AppDbContext _context;

        public DonorService(AppDbContext context)
        {
            _context = context;
        }

        public List<DonorDto> GetAllDonors(string? search)
        {
            // מתחילים בשאילתה (עדיין לא רצים ל-DB)
            var query = _context.Donors.AsQueryable();

            // אם נשלח ערך לחיפוש - מסננים
            if (!string.IsNullOrEmpty(search))
            {
                // חיפוש לפי שם, מייל, או שם של מתנה שהם תרמו
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    d.Email.Contains(search) ||
                    d.Gifts.Any(g => g.Name.Contains(search)) // חיפוש לפי מתנה!
                );
            }

            return query.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
            }).ToList();
        }
        public DonorDto GetDonorById(int id)
        {
            var donor = _context.Donors.Find(id);
            if (donor == null)
            {
                return null;
            }
            return new DonorDto
            {
                Id = donor.Id,
                Name = donor.Name,
                Email = donor.Email,
                Phone = donor.Phone
            };
        }

        public int AddDonor(DonorDto donorDto)
        {
            var newDonor = new Donor
            {
                Name = donorDto.Name,
                Email = donorDto.Email,
                Phone = donorDto.Phone
            };

            _context.Donors.Add(newDonor);
            _context.SaveChanges();

            return newDonor.Id;
        }

        // Implementation for interface method
        public void AddDonor(Donor donor)
        {
            _context.Donors.Add(donor);
            _context.SaveChanges();
        }

        public bool UpdateDonor(DonorDto donorDto)
        {
            var existingDonor = _context.Donors.FirstOrDefault(d => d.Id == donorDto.Id);

            if (existingDonor == null)
            {
                Console.WriteLine($"[DEBUG SERVICE] Donor with ID {donorDto.Id} NOT FOUND in DB.");
                return false;
            }

            // --- מצלמה 2: מה היה קודם ומה יהיה עכשיו? ---
            Console.WriteLine($"[DEBUG SERVICE] Found Donor. Old Name: '{existingDonor.Name}'");
            Console.WriteLine($"[DEBUG SERVICE] Updating to New Name: '{donorDto.Name}'");

            // עדכון הערכים
            existingDonor.Name = donorDto.Name;
            existingDonor.Email = donorDto.Email;
            existingDonor.Phone = donorDto.Phone;

            // --- מצלמה 3: האם ה-DB באמת שמר? ---
            var rowsAffected = _context.SaveChanges();
            Console.WriteLine($"[DEBUG SERVICE] Rows Affected in DB: {rowsAffected}");

            return true;
        }
        public bool DeleteDonor(int id)
        {
            // 1. בדיקה: האם לתורם יש מתנות?
            bool hasGifts = _context.Gifts.Any(g => g.DonorId == id);
            if (hasGifts)
            {
                // זורקים שגיאה עם הטקסט הספציפי שביקשת
                throw new Exception("אין אפשרות למחוק תורם שמשויכות לו מתנות.");
            }

            // 2. חיפוש התורם
            var donor = _context.Donors.FirstOrDefault(d => d.Id == id);

            // אם לא נמצא - נחזיר false (כדי שהקונטרולר יחזיר NotFound)
            if (donor == null) return false;

            // 3. מחיקה
            _context.Donors.Remove(donor);
            _context.SaveChanges();

            return true;
        } 
    
    }
  }