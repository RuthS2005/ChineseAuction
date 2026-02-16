using System.ComponentModel.DataAnnotations;

namespace MechiraSinit.Models
{
    public class Gift
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }=string.Empty;
        [Required]
        public string Description { get; set; }=string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty; 
        [Required]
        public int TicketsSold { get; set; }
        [Required]
        public int Cost {  get; set; }
        [Required]
        public int DonorId {  get; set; }
        public string? WinnerName { get; set; }

        public Donor? Donor { get; set; }
        public List<Purchase> Purchases { get; set; }= new List<Purchase>();

    }
}
