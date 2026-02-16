using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Dto
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        public string Password { get; set; } = string.Empty;
    }
}
    