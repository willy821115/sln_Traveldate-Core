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
                return outlineDetails.ToList(); // 将分割后的数组转换为列表并返回
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

        public List<CCategoryAndTags> loadCategories()
        {
            TraveldateContext db = new TraveldateContext();
            List<CCategoryAndTags> list = new List<CCategoryAndTags>();

            var data_category = db.ProductTagLists
                .GroupBy(c => c.ProductTagDetails.ProductCategory.ProductCategoryName)
                .Select(g =>
                new
                {
                    category = g.Key,
                    tag = g.Select(t => t.ProductTagDetails.ProductTagDetailsName)
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

        public List<CCountryAndCity> loadCountry()
        {
            TraveldateContext db = new TraveldateContext();
            List<CCountryAndCity> list = new List<CCountryAndCity>();
            var data_region = db.ProductLists
                
                .GroupBy(r => r.City.Country.Country)
                .Select(g => new
                {
                    country = g.Key,
                    citys = g.Select(c => c.City.City).Distinct()
                });

            foreach (var c in data_region)
            {
                CCountryAndCity x = new CCountryAndCity();
                x.country = c.country;
                x.citys = c.citys.Select(city =>
                {
                    if (city.Trim().Substring(city.Length - 1, 1) == "縣" || city.Trim().Substring(city.Length - 1, 1) == "市")
                    {
                        return city.Substring(0, city.Length - 1);
                    }
                    return city;
                }).ToList();
                list.Add(x);
            }
            return list;
        }

        public List<string> loadTypes()
        {
            TraveldateContext db = new TraveldateContext();
            List<string> list = new List<string>();
            IEnumerable<string> datas_types = db.ProductLists
             .Select(t => t.ProductType.ProductType);
            list.AddRange(datas_types);
            return list;
        }

        public int TripStock(int tripID)
        {
            TraveldateContext _db = new TraveldateContext();

            var q = _db.Trips.Where(s => s.TripId == tripID).Select(s => new { orders = s.TripDetails.Count, max = s.MaxNum }).FirstOrDefault();
            int stock = (int)q.max - (int)q.orders;
            return stock;
        }


    }
}
