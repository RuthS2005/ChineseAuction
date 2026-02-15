using MechiraSinit.Dto;

namespace MechiraSinit.Services
{
    public interface IPurchaseService
    {
        void AddToCart(NewPurchaseDto dto); // הוספה לסל
        bool Checkout(int userId);
        List<object> GetUserCart(int userId); // (בונוס) צפייה בסל
        List<ReportIncomeDto> GetIncomeReport();
        decimal GetTotalIncome(); // סך הכל כללי
    }
}