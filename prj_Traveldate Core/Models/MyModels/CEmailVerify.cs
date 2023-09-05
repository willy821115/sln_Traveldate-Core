namespace prj_Traveldate_Core.Models.MyModels
{
    public class CEmailVerify
    {
        //製作認證連結 需要的參數

        public string Email { get; set; }
        public string receivePage { get; set; }
        public string scheme { get; set; }
        public string host { get; set; }
    }
}
