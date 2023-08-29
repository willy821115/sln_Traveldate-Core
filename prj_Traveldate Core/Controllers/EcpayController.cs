using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using System.Net.Http.Headers;

namespace prj_Traveldate_Core.Controllers
{
    public class EcpayController : Controller
    {
        TraveldateContext _context = new TraveldateContext();
        private readonly IMemoryCache _cache;

        public EcpayController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        //step4 : 新增訂單
        [HttpPost]
        [Route("api/Ecpay/AddOrders")]
        public string AddOrders([FromBody]CEcpayMI json)
        {
            string num = "0";

            try
            {
                EcpayOrder ecOrder = _context.EcpayOrders.FirstOrDefault();
                //if (ecOrder != null)
                //{
                //    ecOrder.MemberId = json.MerchantID;
                //    ecOrder.RtnCode = 0; //未付款
                //    ecOrder.RtnMsg = "訂購成功尚未付款";
                //    ecOrder.TradeNo = json.MerchantID.ToString();
                //    ecOrder.TradeAmt = json.TotalAmount;
                //    ecOrder.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
                //    ecOrder.PaymentType = json.PaymentType;
                //    ecOrder.PaymentTypeChargeFee = "0";
                //    ecOrder.TradeDate = json.MerchantTradeDate;
                //    ecOrder.SimulatePaid = 0;
                //}

                _context.SaveChanges();
                num = "OK";
            }
            catch (Exception ex)
            {
                num = ex.ToString();
            }
            return num;
        }

        [HttpPost]
        [Route("api/Ecpay/AddPayInfo")]
        public HttpResponseMessage AddPayInfo(JObject info)
        {
            try
            {
                _cache.Set("MerchantTradeNo", info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        [HttpPost]
        [Route("api/Ecpay/AddAccountInfo")]
        public HttpResponseMessage AddAccountInfo(JObject info)
        {
            try
            {
                _cache.Set("MerchantTradeNo", info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }



        /// step5 : 取得付款資訊，更新資料庫
        [HttpPost]
        public ActionResult PayInfo(IFormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            string temp = id["MerchantTradeNo"]; //寫在LINQ(下一行)會出錯，
            var ecpayOrder = _context.EcpayOrders.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                _context.SaveChanges();

                TempData["ECODID"] = temp;
            }
            //return View("EcpayView", data);
            return RedirectToAction("CompleteOrder","Cart");
        }
        /// step5 : 取得虛擬帳號 資訊
        [HttpPost]
        public ActionResult AccountInfo(IFormCollection id)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            string temp = id["MerchantTradeNo"]; //寫在LINQ會出錯
            var ecpayOrder = _context.EcpayOrders.Where(m => m.MerchantTradeNo == temp).FirstOrDefault();
            if (ecpayOrder != null)
            {
                ecpayOrder.RtnCode = int.Parse(id["RtnCode"]);
                if (id["RtnMsg"] == "Succeeded") ecpayOrder.RtnMsg = "已付款";
                ecpayOrder.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
                ecpayOrder.SimulatePaid = int.Parse(id["SimulatePaid"]);
                _context.SaveChanges();
            }
            return RedirectToAction("CompleteOrder", "Cart");

            //return View("EcpayView", data);
        }
    }
}
