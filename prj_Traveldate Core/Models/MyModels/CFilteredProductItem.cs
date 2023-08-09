using Microsoft.Identity.Client;
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
        public int price { get; set; }
        public byte[]? photo { get; set; }
        public string? date { get; set; }
        public List<string> productTags { get; set; } = new List<string>();
        public string? city { get; set; }
        public string? type { get; set; }
        public double? commentAvgScore { get; set; }
        public int commentCount { get; set; }
        public string strComment { get; set; }
        public int? orederCount { get; set; }


        //public List<ProductList?> product { get; set; }
        //public List<Trip?> trip { get; set; }
        //public ProductPhotoList? photo { get; set; }
        //public ProductTagList? tagsList { get; set; }
        //public ProductTagDetail? tagsDetail { get; set; }
        //public ProductCategory? category { get; set; }
        //public ProductTypeList? type { get; set; }
        //public CommentList? comment { get; set; }

        //public List<int> confirmedId
        //{
        //    get
        //    {
        //        var confirmedIds = _db.Trips
        //            .Where(t=>t.ProductId==t.Product.ProductId
        //            && t.Product.ProductTypeId==1
        //            && t.Product.Discontinued ==false)
        //            .Select(t=>t.ProductId)
        //            .ToList();
        //        return confirmedIds;
        //    }
        //}

    }
}
