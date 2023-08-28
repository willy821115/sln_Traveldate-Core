namespace prj_Traveldate_Core.Models.MyModels
{
    public class CEcpayMI
    {
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ExpireDate { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string ReturnURL { get; set; }
        public string OrderResultURL { get; set; }
        public string PaymentInfoURL { get; set; }
        public string ClientRedirectURL { get; set; }
        public string MerchantID { get; set; }
        public string IgnorePayment { get; set; }
        public string PaymentType { get; set; }
        public string ChoosePayment { get; set; }
        public int EncryptType { get; set; }
    }
}
