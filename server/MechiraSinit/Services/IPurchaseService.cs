using MechiraSinit.Dto;

namespace MechiraSinit.Services
{
    public interface IPurchaseService
    {
        void AddToCart(NewPurchaseDto dto); // הוספה לסל
        bool Checkout(int userId);
        List<object> GetUserCart(int userId); // (בונוס) צפייה בסל
 
    }
}