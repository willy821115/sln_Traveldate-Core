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

        public string loadOutline(int id)
        {
            var outline = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Outline).FirstOrDefault();
            return outline?.ToString();
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
            
            public string loadPlanDescri(int id)
            {
                var planDescription =db.ProductLists.Where(p => p.ProductId == id).Select(p => p.PlanDescription).FirstOrDefault();
                return planDescription;
            }

        public int loadPrice(int id)
        {
            var planPrice = db.Trips.Where(p => p.ProductId == id).Select(t=>t.UnitPrice).FirstOrDefault();
            return (int)planPrice;
        }


    }
}
