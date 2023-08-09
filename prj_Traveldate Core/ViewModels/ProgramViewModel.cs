using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class ProgramViewModel
    {
        private ProductList _prod = null;
        private CProgram _cprogram = null;
        public ProductList product
        {
            get { return _prod; }
            set { _prod = value; }
        }
        public CProgram program
        {
            get { return _cprogram; }
            set { _cprogram = value; }
        }
        public ProgramViewModel()
        {
            _prod = new ProductList();
            _cprogram = new CProgram();
        }
    }
}
