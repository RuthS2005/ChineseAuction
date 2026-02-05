using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MechiraSinit.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        // פרטי המשתמש
        public int UserId { get; set; }
        public User? User { get; set; }

        // פרטי המתנה (זה מה שהיה חסר לסרוויס!)
        public int GiftId { get; set; }
        public Gift? Gift { get; set; }

        // הסטטוס (סל vs שולם)
        public bool IsPaid { get; set; } = false;

        public int Quantity {  get; set; }   

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}