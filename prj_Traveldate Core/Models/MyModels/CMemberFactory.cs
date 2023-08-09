namespace prj_Traveldate_Core.Models.MyModels
{
    public class CMemberFactory
    {
        TraveldateContext db = new TraveldateContext();
        public string couponlistid(int? id)
        {
            // var couponlistid=db.Members.Where(m=>m.MemberId==id).Select(m=>m.MemberId).ToList();

            var datas = from m in db.Members
                        join c in db.Coupons
                        on m.MemberId equals c.MemberId
                        join cl in db.CouponLists
                        on c.CouponListId equals cl.CouponListId
                        where m.MemberId == c.MemberId
                        select new { cl.CouponListId, cl.CouponName, cl.Discount, cl.Description, cl.DueDate };
            return datas.ToString();
        }
    }
}
