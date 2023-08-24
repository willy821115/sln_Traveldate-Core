namespace prj_Traveldate_Core.Models.MyModels
{
    public class CPlatformFactory
    {
        TraveldateContext db = new TraveldateContext();

        //load發放數量，查看coupon裡面有幾個

        public int couponNum(int id)
        {
            var counum = db.Coupons.Where(c=>c.CouponListId == id).Count();
            return counum;
        }
        public int couponUsedNum(int id)
        {
            var couUsednum = db.Orders.Where(o => o.CouponListId == id).Count();
            return couUsednum;
        }

    }
}
