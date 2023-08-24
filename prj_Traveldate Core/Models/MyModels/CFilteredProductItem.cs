using Microsoft.Identity.Client;
using System.Drawing;
using System.Runtime.InteropServices;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class CFilteredProductItem
    {
        #region 會用到的ProductList的屬性
        public int? productID { get; set; }
        public string? productName { get; set; }
        public int? CityID { get; set; }
        public int? ProductTypeID { get; set; }

        public bool? Discontinued { get; set; }
        public string? outlineForSearch { get; set; }
#endregion
        public int? price { get; set; }
        public string? photoPath { get; set; }
        public string? date { get; set; }
        public List<string>? productTags { get; set; } = new List<string>();
        public string? city { get; set; }
        public string? type { get; set; }
        public double? commentAvgScore { get; set; }
        public int? commentCount { get; set; }
        public string? strComment { get; set; }
        public int? orederCount { get; set; }
        public string? strProdStock { get; set; }
        public double? prodStock { get; set; }
    }
}
