using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class CFilteredProductFactory
    {
        public List<int> confirmedId = null;
        TraveldateContext db = null;
        public CFilteredProductFactory() 
        {
            db = new TraveldateContext();
            confirmedId = db.Trips.Include(t=>t.OrderDetails)
               .Where(t => t.Product.ProductId == t.ProductId && t.Product.StatusId == 1 && t.Product.Discontinued == false &&t.Date>DateTime.Now 
               )
               .Select(n => n.ProductId).ToList();
        }
        
        public List<CFilteredProductItem> qureyFilterProductsInfo()
        {
            //var ff = db.Trips.Include(t=>t.OrderDetails).Where(t=>t.OrderDetails.Sum(o => o.Quantity) < t.MaxNum).Select(n=>n.TripId).ToList();
            try
            {
                var datas = db.Trips.Where(t => confirmedId.Contains(t.ProductId)).Include(t => t.Product).Include(t => t.Product.City).Include(t => t.Product.ProductType).GroupBy(t => t.ProductId)
                .Select(g =>
                new
                {
                    id = g.Key,
                    name = g.Select(n => n.Product.ProductName),
                    outlineForSearch = g.Select(n => n.Product.OutlineForSearch),
                    city = g.Select(n => n.Product.City.City).Distinct(),
                    date = g.Select(n => n.Date).Where(d => d.Value > DateTime.Now).Min(),
                    //TODO 要加上註解的地方,但目前資料庫資料不齊全會導致沒有資料可以參考而EXCEPTION
                    tripId = g.Where(d => d.Date.Value > DateTime.Now && (d.OrderDetails.Sum(o => o.Quantity) < d.MaxNum)) // 篩選日期大於今天的行程
                                      .OrderBy(d => d.Date) // 按日期升序排序
                                      .Select(i => (int?)i.TripId) // 選取TripId
                                     .FirstOrDefault() ?? g.OrderBy(i => i.Date).Last().TripId, // 取第一個符合條件的TripId),
                    price = g.Select(n => n.UnitPrice).Min(),
                    type = g.Select(n => n.Product.ProductType.ProductType).Distinct()
                })
                .ToList();

                List<CFilteredProductItem> list = new List<CFilteredProductItem>();
               
                foreach (var p in datas)
                {
                    CFilteredProductItem item = new CFilteredProductItem();
                    item.productID = p.id;
                    item.productName = p.name.FirstOrDefault();
                    if (p.outlineForSearch.FirstOrDefault().Length > 85)
                    {
                        item.outlineForSearch = p.outlineForSearch.FirstOrDefault().Substring(0, 85) + "…";
                    }
                    else
                    {
                        item.outlineForSearch = p.outlineForSearch.FirstOrDefault();
                    }
                    if (p.city.FirstOrDefault().Trim().Substring(p.city.FirstOrDefault().Length - 1, 1) == "縣"
                       || p.city.FirstOrDefault().Trim().Substring(p.city.FirstOrDefault().Length - 1, 1) == "市")
                    {
                        item.city = p.city.FirstOrDefault().Substring(0, p.city.FirstOrDefault().Length - 1);
                    }
                    if (p.date > DateTime.Now)
                    {
                        item.date = p.date.Value.ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        item.date = DateTime.Now.ToShortDateString();
                    }
                   
                    item.price = (int)p.price;
                    item.type = p.type.FirstOrDefault();

                    //標籤
                    var tags = db.ProductTagLists.Where(n => n.ProductId == item.productID)
                        .Select(n => n.ProductTagDetails.ProductTagDetailsName);
                    foreach (var tag in tags)
                    {
                        if (!item.productTags.Contains(tag))
                            item.productTags.Add(tag);
                    }
                    //照片
                    item.photoPath = db.ProductPhotoLists.Where(p => item.productID == p.ProductId).Select(p => p.ImagePath).FirstOrDefault();
                    //評分總分/次數
                    var comments = db.CommentLists.Where(c => c.ProductId == item.productID).Select(c => c.CommentScore);
                    item.commentAvgScore = comments.Average();
                    item.commentCount = comments.Count();
                    item.strComment = comments.Any() ? $"{comments.Average():0.0} ({comments.Count()})" : "尚無評論";
                    //購買次數
                    var buy = db.OrderDetails.Where(o => o.Trip.Product.ProductId == item.productID && o.Order.IsCart==false).Select(o => o.Quantity).Sum();
                    item.orederCount = buy.HasValue ? buy : 0;
                    //trip的剩餘名額
                    CProductFactory prodFactory = new CProductFactory();
                    var strStock = prodFactory.TripStock(p.tripId);
                    double r = Convert.ToDouble(strStock.Split('/')[0]);
                    double m = Convert.ToDouble(strStock.Split('/')[1]);
                    if (r > m)
                    {
                        r = m;
                    }

                    item.prodStock = r / m;
                    if (item.prodStock == 1)
                    {
                        item.strProdStock = $"已額滿   名額:{r}/{m}";
                    }
                    else if (item.prodStock > 0.5)
                    {
                        item.strProdStock = $"即將額滿   名額:{r}/{m}";
                    }
                    else
                    {
                        item.strProdStock = $"名額:{r}/{m}";
                    }
                    list.Add(item);
                }
                return list;
            }
            catch(Exception ex)
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
            
        }
     
        
        public List<CCategoryAndTags> qureyFilterCategories()
        {
            try
            {
                TraveldateContext db = new TraveldateContext();
                List<CCategoryAndTags> list = new List<CCategoryAndTags>();

                var data_category = db.ProductTagLists
                    .Where(c => confirmedId.Contains((int)c.ProductId))
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
                    x.tags = i.tag.ToList();
                    list.Add(x);
                }
                return list;
            }
            catch(Exception ex)
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
           
        }

        public List<CCountryAndCity> qureyFilterCountry()
        {
            try
            {
                TraveldateContext db = new TraveldateContext();
                List<CCountryAndCity> list = new List<CCountryAndCity>();
                var data_region = db.ProductLists
                    .Where(c => confirmedId.Contains((int)c.ProductId))
                    .GroupBy(r => r.City.Country.Country)
                    .Select(g => new
                    {
                        country = g.Key,
                        citys = g.Select(c => c.City.City)
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
            catch (Exception ex)
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
            
        }
        public List<string> qureyFilterTypes()
        {
            try
            {
TraveldateContext db = new TraveldateContext();
            List<string> list = new List<string>();
            IEnumerable<string> datas_types = db.ProductLists
                .Where(t => confirmedId.Contains((int)t.ProductId))
                .Select(t => t.ProductType.ProductType);
            list.AddRange(datas_types);
            return list;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }     
        }
    }
}

