using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prj_Traveldate_Core.ViewModels
{
    public class CForgotPwdViewModel
    {
        [DisplayName("您的E-mail")]
        [Required(ErrorMessage = "E-mail為必填")]
        [EmailAddress(ErrorMessage = "E-mail格式錯誤")]
        public string email { get; set; } 
    }
}
