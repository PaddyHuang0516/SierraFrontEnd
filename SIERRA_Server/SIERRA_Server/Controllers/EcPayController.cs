﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SIERRA_Server.Models.EFModels;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace SIERRA_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
  
    public class EcPayController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EcPayController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult EcPayCheckOut([FromBody] UserRequest request)
        {
            //根據提供的username查找最新一筆訂單資訊
            var latestOrder =  _context.DessertOrders
                                            .Where(o => o.Username == request.Username)
                                            .OrderByDescending(o => o.CreateTime)
                                            .FirstOrDefault();
            var dessertOrderTotal = latestOrder.DessertOrderTotal;

        

            //根據最新訂單的Id獲取所有相應的DessertOrderDetails資料
            var dessertDetails =  _context.DessertOrderDetails.Where(d => d.DessertOrderId == latestOrder.Id).ToList();

            if (!dessertDetails.Any())
            {
                return BadRequest("No dessert details found for the order.");
            }

            //將所有的dessertName使用#組合成一個字符串
            var combinedDessertNames = string.Join("#", dessertDetails.Select(d => d.DessertName));

            var orderId = Guid.NewGuid().ToString("N").Substring(0, 5);


            //需填入你的網址
            var website = "https://8c53-2001-b400-e290-8861-387b-291-d5c6-fbc0.ngrok.io ";
            var order = new Dictionary<string, string>
            {
                //{"HashKey","pwFHCqoQZGmho4w6"},
                //{"HashIV" ,"EkRm7iFT261dpevs"},
                {"MerchantID","3002607" },
                {"MerchantTradeNo",orderId},
                {"MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                {"TotalAmount",dessertOrderTotal.ToString() },
                {"TradeDesc", "好吃的甜點不用華麗鋪張，做出具備撫慰人心效果的甜點"},
                {"ItemName",combinedDessertNames },
                {"ReturnURL", "https://localhost:520/api/EcPay/EcpayReturn"},
                { "OrderResultURL", "https://localhost:520/api/EcPay/EcpayReturn"},
                { "EncryptType",  "1"},
                {"ChoosePayment" ,"Credit"},
                {"PaymentType", "aio"},

            };
            // 檢查碼計算
            string hashKey = "pwFHCqoQZGmho4w6";
            string hashIV = "EkRm7iFT261dpevs";
            string rawString = $"HashKey={hashKey}&" + string.Join("&", order.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}")) + $"&HashIV={hashIV}";
            string urlEncode = WebUtility.UrlEncode(rawString).ToLower();
            string checkMacValue = BitConverter.ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(urlEncode))).Replace("-", "").ToUpper();

            // 加入 CheckMacValue
            order.Add("CheckMacValue", checkMacValue);
            return Ok(order);
        }
        
        [HttpPost("EcpayReturn")]
        
        public IActionResult EcpayReturn([FromForm] EcpayReturnDto info)
        {
            if (info.RtnMsg == "Succeeded")
            {
                



                return Redirect($"http://localhost:5501/Order.html");
            }
            else
            {
                return Ok(info.RtnMsg);
            }

        }
    }

    public class EcpayReturnDto
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        //public string StoreID { get; set; }
        public int RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string TradeNo { get; set; }
        public int TradeAmt { get; set; }
        public string PaymentDate { get; set; }
    }

    public class UserRequest
    {
        public string Username { get; set; }
    }
}
