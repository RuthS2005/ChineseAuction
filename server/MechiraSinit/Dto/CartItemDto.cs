// CartItemDto
using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class CartItemDto
    {
        [Required]
        public int giftId { get; set; } // הוספתי set כדי שיהיה אפשר לאכלס מה-API

        public string giftName { get; set; }

        [Range(1, 100, ErrorMessage = "ניתן לרכוש בין 1 ל-100 כרטיסים למתנה אחת")]
        public int quantity { get; set; }

        public int totalPrice { get; set; }
    }
}

