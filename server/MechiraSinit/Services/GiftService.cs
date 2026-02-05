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

        public List<GiftDto> GetAllGifts()
        {
            return _context.Gifts
                .Select(g => new GiftDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Category = g.Category,
                    ImageUrl = g.Image,
                    Cost = g.Cost,
                    DonorId = g.DonorId
                })
                .ToList();
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
    }
}