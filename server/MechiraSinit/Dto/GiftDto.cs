namespace MechiraSinit.Dto
{
    public class GiftDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // הוספנו קטגוריה
        public string ImageUrl { get; set; } = string.Empty; // עדיף שם ברור כמו ImageUrl
        public int Cost { get; set; } // חובה: מחיר כרטיס

        public int DonorId { get; set; } // חובה!!! המפתח הזר לתורם

        public string WinnerName { get; set; } = string.Empty;

    }
}