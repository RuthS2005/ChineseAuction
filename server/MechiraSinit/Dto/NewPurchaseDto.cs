namespace MechiraSinit.Dto
{
    public class NewPurchaseDto
    {
        public int GiftId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
