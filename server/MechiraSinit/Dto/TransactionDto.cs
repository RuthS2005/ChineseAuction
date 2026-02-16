using System;
using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class TransactionDto
    {
        public int Id { get; set; }

        [Required]
        public int GiftId { get; set; } // הוספתי set כדי לאפשר מיפוי

        [Required(ErrorMessage = "שם הקונה הוא שדה חובה")]
        public string BuyerName { get; set; }

        [Range(1, 1000, ErrorMessage = "כמות כרטיסים חייבת להיות בין 1 ל-1000")]
        public int TicketCount { get; set; }

        public DateTime? Date { get; set; } // שימוש ב-DateTime התקני של C#
    }
}