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
                DonorId = giftDto.DonorId,
                WinnerName = null // מתנה חדשה, אין עדיין זוכה
            };
            _context.Gifts.Add(newGift);
            _context.SaveChanges();
            return newGift.Id;
        }

        public List<GiftDto> GetAllGifts(string? search, string? sort)
        {
            var query = _context.Gifts
                .Include(g => g.Donor)
                .AsQueryable();

            // --- 1. חיפוש ---
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int soldCount))
                {
                    query = query.Where(g => g.TicketsSold >= soldCount);
                }
                else
                {
                    query = query.Where(g =>
                        g.Name.Contains(search) ||
                        (g.Donor != null && g.Donor.Name.Contains(search))
                    );
                }
            }

            // --- 2. מיון ---
            switch (sort)
            {
                case "expensive":
                    query = query.OrderByDescending(g => g.Cost);
                    break;
                case "popular":
                    query = query.OrderByDescending(g => g.TicketsSold);
                    break;
                case "cheap":
                    query = query.OrderBy(g => g.Cost);
                    break;
                default:
                    query = query.OrderBy(g => g.Id);
                    break;
            }

            // המרה ל-DTO
            // שימי לב כמה זה פשוט עכשיו: השם כבר נמצא בתוך המתנה!
            return query.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Category = g.Category,
                ImageUrl = g.Image,
                Cost = g.Cost,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName // <--- פשוט מעתיקים את השם
            }).ToList();
        }

        public bool UpdateGift(int id, GiftDto giftDto)
        {
            var gift = _context.Gifts.Find(id);
            if (gift == null) return false;

            gift.Name = giftDto.Name;
            gift.Description = giftDto.Description;
            gift.Category = giftDto.Category;
            gift.Image = giftDto.ImageUrl;
            gift.Cost = giftDto.Cost;
            gift.DonorId = giftDto.DonorId;
            // לא מעדכנים כאן את WinnerName, כי זה קורה רק בהגרלה

            _context.SaveChanges();
            return true;
        }

        public bool DeleteGift(int id)
        {
            var gift = _context.Gifts
                .Include(g => g.Purchases)
                .FirstOrDefault(g => g.Id == id);

            if (gift == null) return false;

            bool hasFinalizedPurchases = gift.Purchases.Any(p => p.IsPaid);
            if (hasFinalizedPurchases)
            {
                throw new Exception("לא ניתן למחוק מתנה שנרכשו עבורה כרטיסים סופיים.");
            }

            _context.Gifts.Remove(gift);
            _context.SaveChanges();
            return true;
        }

        public Gift GetGiftById(int id)
        {
            return _context.Gifts.FirstOrDefault(g => g.Id == id);
        }

        public User? RunRaffle(int giftId)
        {
            var gift = _context.Gifts
                .Include(g => g.Purchases)
                .ThenInclude(p => p.User) // חייבים לטעון את היוזר כדי לקחת את שמו
                .FirstOrDefault(g => g.Id == giftId);

            if (gift == null) throw new Exception("המתנה לא נמצאה.");

            // בדיקה אם השדה מלא
            if (!string.IsNullOrEmpty(gift.WinnerName))
                throw new Exception("ההגרלה כבר בוצעה למתנה זו!");

            var validPurchases = gift.Purchases.Where(p => p.IsPaid).ToList();

            if (validPurchases.Count == 0)
                throw new Exception("לא נרכשו כרטיסים למתנה זו, אי אפשר להגריל.");

            var random = new Random();
            int index = random.Next(validPurchases.Count);
            var winningPurchase = validPurchases[index];

            // --- כאן השינוי הגדול ---
            // אנחנו שומרים את השם של הזוכה בתוך המתנה
            gift.WinnerName = winningPurchase.User.Name;

            _context.SaveChanges();

            return winningPurchase.User; // מחזירים את האובייקט המלא למקרה שצריך אותו בקונטרולר
        }

        public List<ReportWinnerDto> GetWinnersReport()
        {
            // שולפים את כל המתנות שכבר יש להן זוכה (שהשם לא ריק)
            var gifts = _context.Gifts
                .Where(g => !string.IsNullOrEmpty(g.WinnerName))
                .ToList();

            // הערה: בגלל שמחקנו את ה-ID, קשה יותר להשיג את המייל של הזוכה בדיוק מושלם
            // (כי יכולים להיות שני יוזרים בשם "משה כהן").
            // הפתרון הפשוט כאן הוא להחזיר רק את השם, או לנסות למצוא מייל לפי שם (אופציונלי).
            // כאן עשיתי חיפוש "Best Effort" למייל, אבל זה לא קריטי אם לא צריך.

            // שליפת כל היוזרים לזיכרון כדי למצוא מיילים לפי שמות (אם המערכת קטנה זה בסדר)
            var usersByName = _context.Users
                                      .GroupBy(u => u.Name) // למקרה של כפילויות שמות
                                      .ToDictionary(g => g.Key, g => g.First().Email);

            return gifts.Select(g => new ReportWinnerDto
            {
                GiftName = g.Name,
                WinnerName = g.WinnerName,

                // מנסים למצוא את המייל לפי השם ששמרנו
                WinnerEmail = (g.WinnerName != null && usersByName.ContainsKey(g.WinnerName))
                              ? usersByName[g.WinnerName]
                              : "לא נמצא מייל"
            }).ToList();
        }
    }
}