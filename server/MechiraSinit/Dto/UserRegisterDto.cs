using System.ComponentModel.DataAnnotations;

public class UserRegisterDto
{
    [Required(ErrorMessage = "שם משתמש הוא שדה חובה")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "שם משתמש חייב להיות בין 3 ל-20 תווים")]
    public string Username { get; set; }

    [Required(ErrorMessage = "סיסמה היא שדה חובה")]
    [MinLength(6, ErrorMessage = "סיסמה חייבת להכיל לפחות 6 תווים")]
    public string Password { get; set; }

    [Required(ErrorMessage = "אימייל הוא שדה חובה")]
    [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
    public string Email { get; set; }

    [RegularExpression(@"^\d{9,10}$", ErrorMessage = "מספר טלפון חייב להכיל 9 או 10 ספרות")]
    public string Phone { get; set; } = string.Empty;
}
