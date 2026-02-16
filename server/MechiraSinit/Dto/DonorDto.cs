using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class DonorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "שם תורם הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם חייב להיות בין 2 ל-50 תווים")]
        public string Name { get; set; }

        [Required(ErrorMessage = "כתובת מייל היא חובה")]
        [EmailAddress(ErrorMessage = "כתובת מייל לא תקינה")]
        public string Email { get; set; }

        [Required(ErrorMessage = "מספר טלפון הוא חובה")]
        [RegularExpression(@"^\d{9,10}$", ErrorMessage = "מספר טלפון חייב להכיל 9 או 10 ספרות")]
        public string Phone { get; set; } // שיניתי ל-string לצורך שמירת ה-0 בתחילת המספר
    }
}