using MechiraSinit.Dto;
using MechiraSinit.Models;

namespace MechiraSinit.Services 
{
    public interface IUserService
    {
        // מקבלים קופסה של נתוני הרשמה, מחזירים את ה-ID של המשתמש החדש
        int Register(UserRegisterDto userDto);

        // מקבלים קופסה של נתוני כניסה, מחזירים את המשתמש שנמצא (או null)
        User? Login(UserLoginDto loginDto);
    }
}