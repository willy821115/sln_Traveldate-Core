﻿namespace prj_Traveldate_Core.Models.MyModels
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
        public string fCommentDate { get; set; }
    }
}
