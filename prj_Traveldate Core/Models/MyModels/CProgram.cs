namespace prj_Traveldate_Core.Models.MyModels
{
    public class CProgram
    {
        public List<byte[]> fPhotoList { get; set; }
        public List<string> fTripDate { get; set; }
        public List<string> fOutline { get; set; }
        public List<string> fPlanDescription { get; set; }
        public List<decimal?> fPlanPrice { get; set; }
        public List<string> fTripDetails { get; set; }
        public decimal? fTripPrice { get; set; }
        public List<string> fCommentDate { get; set; }
        public List<string> fComMem { get; set; }
        public List<string> fComMemGender { get; set; }
        public List<int?> fComScore { get; set; }
        public List<string> fComContent { get; set; }
        public List<string> fComTitle { get; set; }
        public string fStatus { get; set; }
        public int fQuantity { get; set; }
        public List<string> fPhotoPath { get; set; }
        public List<string> fTTripPhotoList { get; set; }
        public List<string> fCommentPhotoList { get; set; }
        public List<string> fProdTags { get; set; }
        public List<int> fTripId { get; set; }
        public int floggedInMemberId { get; set; }

        public List<decimal?> fDiscountPlanPrice { get; set; }
        public List<string> fDiscountPriceDate { get; set; }
        public List<string> fMemPic { get; set; }
        public bool fIsFav { get; set; }
        public string fTripStatus { get; set; }
        public List<int?> fMaxnum { get; set; }

    }
}
