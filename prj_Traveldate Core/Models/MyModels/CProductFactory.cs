using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class CProductFactory
    {
        TraveldateContext db = new TraveldateContext();
        public List<string> loadPhotoPath(int id)
        {
            List<string> photopath = db.ProductPhotoLists.Where(p => p.ProductId == id).Select(p => p.ImagePath).ToList();
            return photopath;
        }
      

        public List<byte[]> loadPhoto(int id)
        {
            List<byte[]> photoList = db.ProductPhotoLists
                    .Where(p => p.ProductId == id)
                    .Select(p => p.Photo)
                    .ToList();
            return photoList;
        }

        public string loadTitle(int id)
        {
            var productName = db.ProductLists
                .Where(p => p.ProductId == id)
                .Select(p => p.ProductName)
                .FirstOrDefault();
            return productName?.ToString();
        }

        //Producttags
        public List<string> loadProductTags(int id)
        {
            List<string> tags = db.ProductTagLists.Where(p=>p.ProductId == id).Select(p=>p.ProductTagDetails.ProductTagDetailsName).ToList();
            return tags;
        }



        public List<string> loadOutlineDetails(int id)
        {
            var outline = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Outline).FirstOrDefault();

            if (outline != null)
            {
                string[] outlineDetails = outline.Split('\n');
                return outlineDetails.ToList(); 
            }

            return new List<string>(); // 返回空列表
        }

        public string loadDescription(int id)
        {
            var dspt = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Description).FirstOrDefault();
            return dspt?.ToString();
        }

        public List<string> loadTrip(int id)
        {
            List<DateTime?> tripDates = db.Trips.Where(p => p.ProductId == id &&p.Date>DateTime.Now.AddDays(-7))
                .OrderBy(t => t.Date).Select(t => t.Date).ToList();
            List<string> formattedDates = tripDates.Select(d => d?.ToString("yyyy-MM-dd")).ToList();
            return formattedDates;
        }

        public List<int> loadTripId(int id)
        {
            List<int> tripId = db.Trips.Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7))
                .OrderBy(t => t.Date).Select(t => t.TripId).ToList();
            return tripId;
        }



        //最多可報名可賣數量
        public List<int?> loadQuantityMax(int id)
        {
            List<int?> maxnum = db.Trips.Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7)).OrderBy(t => t.Date).Select(t => t.MaxNum).ToList();
            return maxnum;
        }


        public List<int?> loadStock(int id)
        {
            List<int?> quantities = new List<int?>();
            List<int?> maxnum = db.Trips
                .Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7))
                .OrderBy(t => t.Date)
                .Select(t => t.MaxNum)
                .ToList();

            List<int> tripIds = db.Trips
                .Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7))
                .OrderBy(t => t.Date)
                .Select(t => t.TripId)
                .ToList();

            foreach (int tripId in tripIds)
            {
                int? quantity = db.OrderDetails
                    .Where(o => o.Order.IsCart == false && o.TripId == tripId)
                    .Sum(o => (int?)o.Quantity); // 使用 int? 轉型為可為 null 的整數
                quantities.Add(quantity);
            }

            // 使用 Zip 方法將 maxnum 和 quantities 組合成一個 List<int?>
            List<int?> combinedList = maxnum.Zip(quantities, (m, q) => m - q).ToList();

            return combinedList;
        }

        //最少報名人數
        public List<int?> loadQuantityMin(int id)
        {
            List<int?> maxnum = db.Trips.Where(p => p.ProductId == id).Select(t => t.MinNum).ToList();
            return maxnum;
        }




        public string loadAddress(int id)
        {
            var address = db.ProductLists.Where(p=>p.ProductId==id).Select(p => p.Address).FirstOrDefault();
            return address;
        }
        

        public string loadPlan(int id)
        {
            //Load 方案名稱
            var planName = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.PlanName).FirstOrDefault();
            return planName;
        }

        //Load 方案內容

        public List<string> LoadPlanDescri(int id)
        {
            var planDescription = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.PlanDescription).FirstOrDefault();
            if (planDescription != null)
            {
                string[] planDetails = planDescription.Split('\n');
                return planDetails.ToList(); 
            }
            return new List<string>(); // 返回空列表
        }

        public string loadStatus(int id)
        {
            var maxNums = db.Trips.Where(p => p.ProductId == id).Select(t => t.MaxNum).ToList();
            var tripdate = db.Trips.Where(p => p.ProductId == id).Select(t => t.Date).ToList();

                bool allSoldOut = maxNums.All(maxNum => maxNum == 0);
            bool IsExpired = tripdate.All(tripdate => tripdate < DateTime.Now);
            
                if (allSoldOut)
                {
                    return "已售完";
                }
            else if(IsExpired)
            {
                return "已截止";
            }
                else
                {
                    return "熱賣中";
                }
        }

       

        //Product的縣市顯示再tilte label
        public string loadCity(int id)
        {
            var city = (from p in db.ProductLists
                        join c in db.CityLists on p.CityId equals c.CityId
                        where p.ProductId == id
                        select c.City).FirstOrDefault();
            return city;
        }

        //Comment
        public List<string> loadCommentMem(int id)
        {
            List<string> commember = (from c in db.CommentLists
                            join m in db.Members on c.MemberId equals m.MemberId
                            where c.ProductId == id
                            select m.LastName).ToList();
            return commember;
        }
        public List<string> memgender(int id)
        {
            List<string> membergen = (from c in db.CommentLists
                             join m in db.Members on c.MemberId equals m.MemberId
                             where c.ProductId == id
                             select m.Gender).ToList();
            return membergen;
        }

        public List<string> loadMemPic(int id)
        {
            List<string> mempic = (from c in db.CommentLists
                                      join m in db.Members on c.MemberId equals m.MemberId
                                      where c.ProductId == id
                                      select m.ImagePath).ToList();
            return mempic;
        }
       

        public List<string> loadCommentDate(int id)
        {
            List<DateTime?> comdate = db.CommentLists.Where(c=>c.ProductId ==id).Select(c=>c.Date).ToList();
            List<string> comdatetime = comdate.Select(d => d?.ToString("yyyy-MM-dd")).ToList();
            return comdatetime;
        }

        public List<int?> loadcommentScore(int id)
        {
            List<int?> comScore = db.CommentLists.Where(c=>c.ProductId==id).Select(c=>c.CommentScore).ToList();
            return comScore;
        }

        public List<string> loadCommentContent(int id)
        {
            List<string> comcontent = db.CommentLists.Where(c => c.ProductId == id).Select(c=>c.Content).ToList();
            return comcontent;
        }

        public List<string> loadCommentTitle(int id)
        {
            List<string> comtiltle = db.CommentLists.Where(c => c.ProductId == id).Select(c => c.Title).ToList();
            return comtiltle;
        }

        public List<string> loadCommentPhotoPath(int id)
        {
            List<string> comphoto = (from t in db.CommentLists
                                     join c in db.CommentPhotoLists on t.CommentId equals c.CommentId
                                     where t.ProductId == id
                                     select c.ImagePath).ToList();
            return comphoto;
        }

        public List<CCommentViewModel> LoadCommentsForProduct(int id)
        {

            List<CCommentViewModel> comments = db.CommentLists
                .Where(c => c.ProductId == id)
                .OrderByDescending(c=>c.Date)
                .Select(c => new CCommentViewModel
                {
                    ComTitle = c.Title,
                    MemLastName = c.Member.LastName,
                    MemGender = c.Member.Gender,
                    ComDate = c.Date,
                    ComScore = c.CommentScore,
                    Content = c.Content,
                    MemPhotoPath = c.Member.ImagePath,
                    ComPhotoPath = string.Join(",", c.CommentPhotoLists.Select(photo => photo.ImagePath))
                })
                .ToList();

            return comments;
        }





        //loadTripPrice原價
        public List<decimal?> loadPlanprice(int id)
        {
            List<decimal?> price = db.Trips.Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7)).OrderBy(t=>t.Date).Select(t => t.UnitPrice).ToList();
            return price;
        }
        //load優惠金額
        public List<decimal?> loadDiscountPrice(int id)
        {
            List<decimal?> disprice = db.Trips.Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7)).OrderBy(t => t.Date).Select(t =>t.Discount).ToList();
            return disprice;
        }
        //優惠價截止日期
        public List<string> loadDiscountPriceDate(int id)
        {
            List<DateTime?> pricedisDates = db.Trips.Where(p => p.ProductId == id && p.Date > DateTime.Now.AddDays(-7)).OrderBy(t => t.Date).Select(t => t.DiscountExpirationDate).ToList();
            List<string> formatPriceDates = pricedisDates.Select(d => d?.ToString("yyyy-MM-dd")).ToList();
            return formatPriceDates;
        }


        //多少錢起的價格
        public decimal? loadPlanpriceStart(int id)
        {
            decimal? price = db.Trips.Where(p => p.ProductId == id).OrderBy(t=>t.UnitPrice).Select(t => t.UnitPrice).FirstOrDefault();
            return price;
        }


        //loadtripdetail
        public List<string> loadTripdetails(int id)
        {
            List<string> tripdetail = db.TripDetails.Where(td => td.ProductId == id).OrderBy(t => t.TripDay).Select(t => t.TripDetail1).ToList();
            return tripdetail;
        }
        public List<string> loadTripPhoto(int id)
        {
            List<string> tripphoto = db.TripDetails.Where(td => td.ProductId == id).OrderBy(t => t.TripDay).Select(t => t.ImagePath).ToList();
            return tripphoto;
        }



        public List<CCategoryAndTags> loadCategories()
        {
            TraveldateContext db = new TraveldateContext();
            List<CCategoryAndTags> list = new List<CCategoryAndTags>();

            var data_category = db.ProductTagDetails
                .GroupBy(c => c.ProductCategory.ProductCategoryName)
                .Select(g =>
                new
                {
                    category = g.Key,
                    tag = g.ToList()

                }) ;
            foreach (var i in data_category.ToList())
            {
                CCategoryAndTags x = new CCategoryAndTags();
                x.productTags = i.tag;
                 x.category = i.category;
                
                list.Add(x);
            }
            return list;
        }

        public List<string> loadCountries()
        {
            TraveldateContext db = new TraveldateContext();
            List<string> list = db.CountryLists.Select(c=>c.Country).ToList();
            return list;
        }

        public List<CityList> loadCities()
        {
            TraveldateContext db = new TraveldateContext();
            List<CityList> list = db.CityLists.ToList();
            return list;
        }
        public List<string> loadStauts()
        {
            TraveldateContext db = new TraveldateContext();
            var list = db.Statuses.Select(p => p.Status1).ToList();
            return list;
        }
        public List<ProductTypeList> loadTypes()
        {
            TraveldateContext db = new TraveldateContext();
             var list= db.ProductTypeLists.ToList();
            return list;
        }

        public string TripStock(int tripID)
        {
            TraveldateContext _db = new TraveldateContext();
             var q = _db.Trips.Where(s => s.TripId == tripID).Select(s => new { orders = s.OrderDetails.Where(o=>o.Order.IsCart==false).Sum(o=>o.Quantity), max = s.MaxNum }).FirstOrDefault();
            string result = $"{q.orders}/{q.max}";
            return result;
        }

        public int TripDays(int productID) 
        {
            TraveldateContext _db = new TraveldateContext();
            var q = _db.TripDetails.Where(t=>t.Product.ProductId==productID).Count();
            return q;
        }

        public List<Company> loadCompanies()
        {
            TraveldateContext db = new TraveldateContext();
            var list = db.Companies.ToList();
            return list;
        }

      

    }
}
