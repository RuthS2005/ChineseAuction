using MechiraSinit.Data;
using MechiraSinit.Dto;
using MechiraSinit.Models;
using Microsoft.EntityFrameworkCore; // חובה בשביל Include

namespace MechiraSinit.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;

        public PurchaseService(AppDbContext context)
        {
            _context = context;
        }

        public void AddToCart(NewPurchaseDto dto)
        {
            // 1. שליפת המתנה
            var gift = _context.Gifts.FirstOrDefault(g => g.Id == dto.GiftId);

            if (gift == null)
                throw new Exception("המתנה לא קיימת");
        
            if (gift.WinnerUserId != null && gift.WinnerUserId > 0)
                
                throw new Exception("ההגרלה הסתיימה, לא ניתן לרכוש כרטיסים");

            // 2. יצירת הכרטיסים
            for (int i = 0; i < dto.Quantity; i++)
            {
                var purchase = new Purchase
                {
                    UserId = dto.UserId,
                    GiftId = dto.GiftId,
                    IsPaid= false, 
                    PurchaseDate = DateTime.Now // כדאי לשמור תאריך
                };
                _context.Purchases.Add(purchase);
            }

            // 3. שמירה ב-DB (החלק שהיה חסר)
            _context.SaveChanges();
        }

        public void Checkout(int userId)
        {
            // שולפים את כל מה שבסל ולא שולם
            var cartItems = _context.Purchases
                .Where(p => p.UserId == userId && !p.IsPaid)
                .ToList();

            if (cartItems.Count == 0)
                throw new Exception("הסל ריק");

            // מעדכנים סטטוס
            foreach (var item in cartItems)
            {
                item.IsPaid = true;
            }

            _context.SaveChanges();
        }

        public List<object> GetUserCart(int userId)
        {
            var items = _context.Purchases
                .Include(p => p.Gift) // טוען את פרטי המתנה
                .Where(p => p.UserId == userId && !p.IsPaid) // רק מה שלא שולם
                .Select(p => new
                {
                    PurchaseId = p.Id,
                    // שימוש בסימן שאלה ? למקרה ש-Gift הוא null (למרות ה-Include)
                    GiftName = p.Gift != null ? p.Gift.Name : "Unknown",
                    Price = p.Gift != null ? p.Gift.Cost : 0,
                    Image = p.Gift != null ? p.Gift.Image : "" // תיקון: השתמשנו ב-Image
                })
                .ToList<object>();

            return items;
        }
    }
}