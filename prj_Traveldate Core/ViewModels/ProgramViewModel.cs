using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class ProgramViewModel
    {
        private ProductList _prod = null;
        private CProgram _cprogram = null;
        private CityList _city = null;
        private CommentList _commentlist = null;
        private Member _member = null;
        private Trip _trip = null;

        public List<CCommentViewModel> comment { get; set; }

        public ProductList product
        {
            get { return _prod; }
            set { _prod = value; }
        }
        public Trip trip
        {
            get { return _trip; }
            set { _trip = value; }
        }
        public CProgram program
        {
            get { return _cprogram; }
            set { _cprogram = value; }
        }
        public CityList city
        {
            get { return _city; }
            set { _city = value; }
        }
        public CommentList commentlist
        {
            get { return _commentlist; }
            set { _commentlist = value; }
        }

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }


        public ProgramViewModel()
        {
            _prod = new ProductList();
            _cprogram = new CProgram();
            _city = new CityList();
            _commentlist = new CommentList();
            _member = new Member();
            _trip = new Trip();
        }
    }
}
