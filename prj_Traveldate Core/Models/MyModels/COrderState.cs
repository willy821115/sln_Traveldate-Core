namespace prj_Traveldate_Core.Models.MyModels
{
    public class COrderState
    {
        public List< CProductWrap> list{get; set;}

        public int orderQuantity { get; set;}

        public decimal turnover { get; set; }

        public List<string> top3product { get; set; }
    }
}
