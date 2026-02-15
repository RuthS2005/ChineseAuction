using MechiraSinit.Data;
using MechiraSinit.Dto;
using MechiraSinit.Models;
using Microsoft.EntityFrameworkCore;

namespace MechiraSinit.Services
{
    public class GiftService : IGiftService
    {
        private readonly AppDbContext _context;

        public GiftService(AppDbContext context)
        {
            _context = context;
        }

        public int AddGift(GiftDto giftDto)
        {
            var newGift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Category = giftDto.Category,
                Image = giftDto.ImageUrl,
                Cost = giftDto.Cost,
                DonorId = giftDto.DonorId
            };
            _context.Gifts.Add(newGift);
            _context.SaveChanges();
            return newGift.Id;
        }
        // הוספנו פרמטרים: טקסט לחיפוש, וסוג מיון
        public List<GiftDto> GetAllGifts(string? search, string? sort)
        {
            var query = _context.Gifts
                .Include(g => g.Donor) // חייב את התורם בשביל החיפוש
                .AsQueryable();

            // --- 1. חיפוש (Filtering) ---
            if (!string.IsNullOrEmpty(search))
            {
                // אם החיפוש הוא מספר - נחפש לפי כמות כרטיסים שנמכרו
                if (int.TryParse(search, out int soldCount))
                {
                    query = query.Where(g => g.TicketsSold >= soldCount);
                }
                else
                {
                    // אחרת - נחפש לפי שם מתנה או שם תורם
                    query = query.Where(g =>
                        g.Name.Contains(search) ||
                        (g.Donor != null && g.Donor.Name.Contains(search))
                    );
                }
            }

            // --- 2. מיון (Sorting) ---
            switch (sort)
            {
                case "expensive": // הכי יקר
                    query = query.OrderByDescending(g => g.Cost);
                    break;
                case "popular": // הכי נרכש
                    query = query.OrderByDescending(g => g.TicketsSold);
                    break;
                default: // ברירת מחדל (למשל לפי ID או שם)
                    query = query.OrderBy(g => g.Id);
                    break;
            }

            return query.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Category = g.Category,
                ImageUrl = g.Image,
                Cost = g.Cost,
                DonorId = g.DonorId,

                // 👇👇👇 התיקון: שליפת שם הזוכה 👇👇👇
                // אנחנו אומרים: "לך לטבלת Users, תמצא את מי שה-ID שלו שווה ל-WinnerUserId של המתנה, ותביא את השם שלו"
                WinnerName = g.WinnerUserId != null
             ? _context.Users
                 .Where(u => u.Id == g.WinnerUserId)
                 .Select(u => u.Name)
                 .FirstOrDefault()
             : null
            }).ToList();
        }
        public bool UpdateGift(int id, GiftDto giftDto)
        {
            var gift = _context.Gifts.Find(id);
            if (gift == null)
            {
                return false;
            }

            // Update allowed fields
            gift.Name = giftDto.Name;
            gift.Description = giftDto.Description;
            gift.Category = giftDto.Category;
            gift.Image = giftDto.ImageUrl;
            gift.Cost = giftDto.Cost;
            gift.DonorId = giftDto.DonorId;

            _context.SaveChanges();
            return true;
        }
        public bool DeleteGift(int id)
        {
            var gift = _context.Gifts.FirstOrDefault(g => g.Id == id);
            if (gift == null) return false;

            _context.Gifts.Remove(gift);
            _context.SaveChanges();
            return true;
        }
        public User? RunRaffle(int giftId)
        {
            // 1. שליפת המתנה + רשימת הרכישות + פרטי המשתמשים
            var gift = _context.Gifts
                .Include(g => g.Purchases)
                .ThenInclude(p => p.User) // חשוב! טוען את היוזר כדי שנוכל להחזיר אותו
                .FirstOrDefault(g => g.Id == giftId);

            // בדיקות תקינות
            if (gift == null)
                throw new Exception("המתנה לא נמצאה.");

            if (gift.WinnerUserId != null)
                throw new Exception("ההגרלה כבר בוצעה למתנה זו!");

            // 2. סינון: לוקחים רק כרטיסים ששולמו (IsPaid = true)
            var validPurchases = gift.Purchases.Where(p => p.IsPaid).ToList();

            if (validPurchases.Count == 0)
                throw new Exception("לא נרכשו כרטיסים למתנה זו, אי אפשר להגריל.");

            // 3. הגרלה רנדומלית
            var random = new Random();
            int index = random.Next(validPurchases.Count); // בוחר מספר בין 0 לכמות הכרטיסים
            var winningPurchase = validPurchases[index];

            // 4. עדכון הזוכה ושמירה
            gift.WinnerUserId = winningPurchase.UserId;
            _context.SaveChanges();

            return winningPurchase.User; // מחזירים את המשתמש המאושר
        }
        public List<ReportWinnerDto> GetWinnersReport()
        {
            // שולפים את כל המתנות, כולל המידע על המנצח (אם יש)
            var gifts = _context.Gifts
                .Include(g => g.Purchases) // כדי לגשת ליוזר דרך הרכישה הזוכה? לא, יש לנו WinnerUserId
                                           // רגע, במודל שלנו שמרנו WinnerUserId.
                                           // אז נצטרך לעשות Join או לשלוף את היוזר.
                                           // הדרך הכי קלה: נשתמש ב-Join ידני או נשנה את המודל שיכלול Navigation Property ל-User Winner.
                                           // נניח כרגע שאין Navigation Property ישיר, אז נעשה את זה חכם:
                .ToList();

            // נשלוף את כל המשתמשים כדי לחבר שמות (או שנעשה את זה בשאילתה אחת אם נוסיף קשר ב-DB)
            var users = _context.Users.ToDictionary(u => u.Id, u => u);

            return gifts.Select(g => new ReportWinnerDto
            {
                GiftName = g.Name,
                WinnerName = g.WinnerUserId.HasValue && users.ContainsKey(g.WinnerUserId.Value)
                             ? $"{users[g.WinnerUserId.Value].Name} "
                             : "טרם בוצעה הגרלה",
                WinnerEmail = g.WinnerUserId.HasValue && users.ContainsKey(g.WinnerUserId.Value)
                             ? users[g.WinnerUserId.Value].Email
                             : ""
            }).ToList();
        }

    }
}