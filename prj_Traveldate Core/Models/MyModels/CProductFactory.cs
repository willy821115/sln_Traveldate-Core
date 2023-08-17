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
        //揪團的地址

        public List<string> loadForumAddress(int id)
        {
            List<string> forumaddress = db.ScheduleLists.Include(s => s.Trip.Product).Where(s => s.ForumListId == id).Select(p => p.Trip.Product.Address).ToList();

            return forumaddress;
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
        public string loadCommentMem(int id)
        {
            var commember = (from c in db.CommentLists
                            join m in db.Members on c.MemberId equals m.MemberId
                            where c.ProductId == id
                            select m.LastName).FirstOrDefault();
            return commember;
        }


        //loadTripPrice
        public List<decimal?> loadPlanprice(int id)
        {
            List<decimal?> price = db.Trips.Where(p => p.ProductId == id).Select(t => t.UnitPrice).ToList();
            return price;
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
                    tag = g.Select(t => t.ProductTagDetailsName)
                });
            foreach (var i in data_category)
            {
                CCategoryAndTags x = new CCategoryAndTags();
                x.category = i.category;
                x.tags = i.tag;
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

        public List<string> loadCities()
        {
            TraveldateContext db = new TraveldateContext();
            List<string> list = db.CityLists.Select(c => c.City).ToList();
            return list;
        }
        public List<string> loadStauts()
        {
            TraveldateContext db = new TraveldateContext();
            var list = db.Statuses.Select(p => p.Status1).ToList();
            return list;
        }
        public List<string> loadTypes()
        {
            TraveldateContext db = new TraveldateContext();
             var list= db.ProductTypeLists.Select(p=>p.ProductType).ToList();
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
