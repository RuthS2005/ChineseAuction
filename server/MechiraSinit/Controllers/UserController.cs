using MechiraSinit.Dto;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens; // 1. קריטי לטוקן
using System.IdentityModel.Tokens.Jwt; // 2. קריטי לטוקן
using System.Security.Claims; // 3. קריטי לפרטי המשתמש
using System.Text;

namespace MechiraSinit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config; // הגישה לקובץ ההגדרות (appsettings)

        // הוספנו את config לבנאי
        public UsersController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto userDto)
        {
            try
            {
                var newId = _userService.Register(userDto);
                return Ok(new { Id = newId, Message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            var user = _userService.Login(loginDto);

            if (user == null)
            {
                return Unauthorized(new { Message = "Email or password incorrect" });
            }

            // --- יצירת הטוקן ---
            var tokenString = GenerateToken(user);

            // מחזירים את הטוקן ללקוח
            return Ok(new
            {
                Token = tokenString,
                id = user.Id,
                name = user.Name,
                role = user.Role
            });
        }

        // הפונקציה שמייצרת את הטוקן המוצפן
        private string GenerateToken(MechiraSinit.Models.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // ה"קלאיימס" הם הפרטים שכתובים על תעודת הזהות
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ה-ID של המשתמש
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // תוקף לשעה
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}