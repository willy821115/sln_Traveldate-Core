namespace prj_Traveldate_Core.Models.MyModels
{
    public class CEmailVerify
    {
        //進行一個寄信的動作 需要的參數

        public string Email { get; set; }
        public string mailContent { get; set; }
        public string mailSubject { get; set; }
        public string receivePage { get; set; }
        public string linkText { get; set; }
        public string scheme { get; set; }
        public string host { get; set; }
    }
}
