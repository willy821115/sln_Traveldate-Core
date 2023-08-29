namespace prj_Traveldate_Core.ViewModels
{
    public class CHomeViewModel
    {
        public int productId { get; set; }
        public string ImagePath { get; set; }

        public string productName { get; set; }

        public decimal unitPrice { get; set; }

        public double? commentScore { get; set; }

        public string commentScoreString { get; set; }
    }
}
