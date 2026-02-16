using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class GiftDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "שם המתנה הוא שדה חובה")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "תיאור המתנה הוא שדה חובה")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "קטגוריה היא שדה חובה")]
        public string Category { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "מחיר כרטיס הוא חובה")]
        [Range(1, 10000, ErrorMessage = "מחיר כרטיס חייב להיות בין 1 ל-10,000")]
        public int Cost { get; set; }

        [Required(ErrorMessage = "חובה לקשר תורם למתנה")]
        public int DonorId { get; set; }

        public string WinnerName { get; set; } = string.Empty;
    }
}