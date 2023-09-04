using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class COrderState
    {
        public List<CProductDetailViewModel>? ProductDetail { get; set; }
        public int? orderQuantity { get; set;}
        public string? turnover { get; set; }
        public List<string>? top3product { get; set; }

        public List<ProductTypeList> types { get; set; }

        public List<string> status { get; set; }

        public int OrderCount(int companyID) 
        {
        TraveldateContext _db=new TraveldateContext();
            var q = _db.OrderDetails.Where(p => p.Trip.Product.CompanyId == companyID);
            return q.Count();
        }

        public decimal Turnover(int companyID)
        {
            TraveldateContext _db = new TraveldateContext();
            var q = _db.OrderDetails.Where(p => p.Trip.Product.CompanyId == companyID).Select(p => new { ordercount = p.Quantity, price = p.Trip.UnitPrice });
            decimal turnover = 0;
            foreach (var item in q) 
            {
                turnover += (decimal)item.price * (decimal)item.ordercount;
             }
            
            return turnover;
        }

        public List<string> Top3(int companyID) 
        {
            TraveldateContext _db = new TraveldateContext();
            var q = _db.OrderDetails.Where(t => t.Trip.Product.Company.CompanyId == companyID).OrderByDescending(o => o.Quantity * o.Trip.UnitPrice).Select(t => t.Trip.Product.ProductName).Distinct().Take(3);

            return q.ToList();
        }
    }
}
