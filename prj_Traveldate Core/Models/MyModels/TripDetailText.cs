namespace prj_Traveldate_Core.Models.MyModels
{
    public class TripDetailText
    {
        public string? TripDetail { get; set; }

        public string? ImagePath { get; set; }

        public int? TripDay { get; set; }

        public int? TripDetailId { get; set; }

        public  IFormFile? photo  { get; set; }

    }
}
