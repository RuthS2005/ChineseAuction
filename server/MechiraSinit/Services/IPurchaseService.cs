using MechiraSinit.Dto;

namespace MechiraSinit.Services
{
    public interface IPurchaseService
    {
        void AddToCart(NewPurchaseDto dto); // הוספה לסל
        void Checkout(int userId); // ביצוע תשלום על כל הסל
        List<object> GetUserCart(int userId); // (בונוס) צפייה בסל
    }
}