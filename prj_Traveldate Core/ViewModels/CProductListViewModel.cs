﻿using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class CProductListViewModel
    {
        public List<CProductWrap> list { get; set; }

        public List<string> types { get; set; }

        public List<string> status { get; set; }
    }
}
