using MechiraSinit.Data;
using MechiraSinit.Dto;
using MechiraSinit.Models;

namespace MechiraSinit.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public int Register(UserRegisterDto userDto)
        {
            // 1. בדיקה אם המייל קיים
            var exists = _context.Users.Any(u => u.Email == userDto.Email);
            if (exists)
            {
                throw new Exception("Email already exists");
            }

            // 2. יצירת אובייקט User
            var newUser = new User
            {
                Name = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password, // הערה: במציאות מצפינים כאן סיסמה!
                Phone = userDto.Phone,
                // הדרך הנכונה לעדכן את התפקיד ב-Service
                Role = UserRole.Customer
            };

            // 3. שמירה
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser.Id;
        }

        public User? Login(UserLoginDto loginDto)
        {
            // חיפוש משתמש שגם המייל וגם הסיסמה תואמים
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == loginDto.Email &&
                u.Password == loginDto.Password);

            return user; // יחזיר את המשתמש או null אם לא מצא
        }
    }
}