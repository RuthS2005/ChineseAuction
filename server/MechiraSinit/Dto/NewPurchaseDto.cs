// NewPurchaseDto
using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class NewPurchaseDto
    {
        [Required]
        public int GiftId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(1, 100, ErrorMessage = "הכמות חייבת להיות לפחות 1")]
        public int Quantity { get; set; } = 1;
    }
}