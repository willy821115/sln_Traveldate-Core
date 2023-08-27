namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCartItem
    {
        public int orderDetailID { get; set; }
        public int? tripID { get; set; }
        public int productID { get; set; }
        public string? planName { get; set; }
        public string? date { get; set; }
        public int? quantity { get; set; }
        public byte[]? photo { get; set; }
        public string? ImagePath { get; set; }
        public decimal? unitPrice { get; set; }
        public decimal? discount { get; set; }
        public bool favo { get; set; }
        public int? ProductTypeID { get; set; }
    }
}
