using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MechiraSinit.Dto
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int GiftId { get; }
        public string BuyerName { get; set; }
        public int TicketCount { get; set; }
        public Date? Date { get; set; }
    }
}
