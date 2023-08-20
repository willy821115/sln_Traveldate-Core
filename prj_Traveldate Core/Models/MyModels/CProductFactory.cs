using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class CProductFactory
    {
        TraveldateContext db = new TraveldateContext();
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

        public int loadPrice(int id)
        {
            var planPrice = db.Trips.Where(p => p.ProductId == id).Select(t=>t.UnitPrice).FirstOrDefault();
            return (int)planPrice;
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


        //loadTripPrice
        public List<decimal?> loadPlanprice(int id)
        {
            List<decimal?> price = db.Trips.Where(p => p.ProductId == id).Select(t => t.UnitPrice).ToList();
            return price;
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

        public int TripStock(int tripID)
        {
            TraveldateContext _db = new TraveldateContext();
             var q = _db.Trips.Where(s => s.TripId == tripID).Select(s => new { orders = s.OrderDetails.Count, max = s.MaxNum }).FirstOrDefault();
            int stock = (int)q.max - (int)q.orders;
            return stock;
        }

        public int TripDays(int productID) 
        {
            TraveldateContext _db = new TraveldateContext();
            var q = _db.TripDetails.Where(t=>t.Product.ProductId==productID).Count();
            return q;
        }


    }
}
