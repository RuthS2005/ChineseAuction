namespace MechiraSinit.Dto
{
    public class ReportIncomeDto
    {
        public string GiftName { get; set; } = string.Empty;
        public int SalesCount { get; set; } // כמה כרטיסים נמכרו
        public decimal TotalIncome { get; set; } // כמה כסף נכנס מהמתנה הזו
    }
}