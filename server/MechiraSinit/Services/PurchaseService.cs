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

        public bool Checkout(int userId)
        {
            // 1. שליפת כל הפריטים של המשתמש שעדיין לא טופלו (נניח שסטטוס false אומר "בעגלה")
            // הערה: תוודאי שבטבלה שלך יש עמודה כמו Status, IsPaid, או Completed
            // אם אין, תצטרכי להוסיף או להשתמש בלוגיקה אחרת. כאן אני מניח ש-Status ברירת מחדל הוא false.
            var cartItems = _context.Purchases
                                    .Where(p => p.UserId == userId && p.IsPaid==false)
                                    .ToList();

            if (cartItems.Count == 0)
            {
                return false; // אין מה לקנות
            }

            // 2. עדכון הסטטוס ל-"שולם" (true)
            foreach (var item in cartItems)
            {
                item.IsPaid = true;
                // אופציונלי: עדכון תאריך הרכישה לרגע זה
                // item.PurchaseDate = DateTime.Now; 
            }

            // 3. שמירה במסד הנתונים
            _context.SaveChanges();
            return true;
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
        public List<ReportIncomeDto> GetIncomeReport()
        {
            // חישוב הכנסות לכל מתנה
            // אנחנו מסתכלים על טבלת הרכישות, מקבצים לפי GiftId
            var report = _context.Purchases
                .Where(p => p.IsPaid) // רק מה ששולם!
                .Include(p => p.Gift)
                .GroupBy(p => p.Gift.Name)
                .Select(group => new ReportIncomeDto
                {
                    GiftName = group.Key,
                    SalesCount = group.Count(),
                    // סכום: מספר הפריטים * מחיר המתנה (מניחים שלכל הפריטים בקבוצה אותו מחיר)
                    // הערה: עדיף לקחת את המחיר מהמתנה עצמה
                    TotalIncome = group.Sum(p => p.Gift.Cost)
                })
                .ToList();

            return report;
        }

        public decimal GetTotalIncome()
        {
            return _context.Purchases
                .Where(p => p.IsPaid)
                .Sum(p => p.Gift.Cost);
        }
    }
}