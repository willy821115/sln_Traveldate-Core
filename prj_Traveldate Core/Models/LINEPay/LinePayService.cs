using ECPAYTEST.Models;
using ECPAYTEST.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using prj_Traveldate_Core.Models.MyModels;
using System.Security.Cryptography;
using System.Text;

namespace ECPAYTEST.Domain
{
    public class LinePayService
    {
        private static HttpClient client;
        private readonly JsonProvider _jsonProvider;
        private readonly string channelId = "2000449327";
        private readonly string channelSecretKey = "04baaaac8b6014bbf73a5bb4f3465f73";
        private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";
        private readonly IMemoryCache _cache;

        public LinePayService(IMemoryCache memoryCache)
        {
            client = new HttpClient();
            _jsonProvider = new JsonProvider();
            _cache = memoryCache;
        }

        // 送出建立交易請求至 Line Pay Server
        public async Task<PaymentResponseDto> SendPaymentRequest(PaymentRequestDto dto)
        {
            var json = _jsonProvider.Serialize(dto);
            // 產生 GUID Nonce
            var nonce = Guid.NewGuid().ToString();
            // 要放入 signature 中的 requestUrl
            var requestUrl = "/v3/payments/request";

            //使用 channelSecretKey & requestUrl & jsonBody & nonce 做簽章
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            // 帶入 Headers
            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

            var response = await client.SendAsync(request);
            var linePayResponse = _jsonProvider.Deserialize<PaymentResponseDto>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(nonce);
            Console.WriteLine(signature);

            return linePayResponse;
        }

        // 取得 transactionId 後進行確認交易
        public async Task<PaymentConfirmResponseDto> ConfirmPayment(string transactionId, string orderId) //加上 OrderId 去找資料
        {
            PaymentConfirmDto dto = new PaymentConfirmDto();
            dto.Currency = "TWD";
            dto.Amount = _cache.Get<Int32>("amount");

            var json = _jsonProvider.Serialize(dto);

            var nonce = Guid.NewGuid().ToString();
            var requestUrl = string.Format("/v3/payments/{0}/confirm", transactionId);
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(linePayBaseApiUrl + requestUrl, transactionId))
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

            var response = await client.SendAsync(request);
            var responseDto = _jsonProvider.Deserialize<PaymentConfirmResponseDto>(await response.Content.ReadAsStringAsync());
            return responseDto;
        }
        public async void TransactionCancel(string transactionId)
        {
            //使用者取消交易則會到這裏。
            Console.WriteLine($"訂單 {transactionId} 已取消");
        }

    }
}
