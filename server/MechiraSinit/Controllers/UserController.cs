using MechiraSinit.Dto;
using MechiraSinit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly ILogger<UsersController> _logger; // 1. משתנה לוגר

    // 2. הזרקה בבנאי
    public UsersController(IUserService userService, IConfiguration config, ILogger<UsersController> logger)
    {
        _userService = userService;
        _config = config;
        _logger = logger;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegisterDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // לוג: מתעדים מי מנסה להירשם (בלי סיסמה!)
        _logger.LogInformation("ניסיון הרשמה עבור אימייל: {Email}", userDto.Email);

        try
        {
            var newId = _userService.Register(userDto);

            _logger.LogInformation("משתמש נרשם בהצלחה. ID: {Id}", newId);
            return Ok(new { Id = newId, Message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            // שגיאה נפוצה: המייל כבר קיים במערכת
            _logger.LogWarning(ex, "שגיאה בהרשמה עבור {Email}", userDto.Email);
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _logger.LogInformation("ניסיון התחברות: {Email}", loginDto.Email);

        var user = _userService.Login(loginDto);

        if (user == null)
        {
            // לוג אזהרה - מישהו מנסה לנחש סיסמה או טעה
            _logger.LogWarning("כישלון בהתחברות (פרטים שגויים) עבור: {Email}", loginDto.Email);
            return Unauthorized(new { Message = "Email or password incorrect" });
        }

        _logger.LogInformation("התחברות מוצלחת: {Email}, תפקיד: {Role}", user.Email, user.Role);

        return Ok(new
        {
            Token = GenerateToken(user),
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

        // ה"קלאיימס" הם הפרטים שכתובים על תעודת הזהות הדיגיטלית
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