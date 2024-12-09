using System.Security.Policy;
using WebBanDongHo.Models;

namespace WebBanDongHo.Helper
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration config;

        public VnPayService(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            //Build URL for VNPAY
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", config["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", config["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", config["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.TotalMoney * 100).ToString());
            //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            //là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", config["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));

            vnpay.AddRequestData("vnp_Locale", config["Vnpay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", config["Vnpay:PagmentBackReturnUrl"]);

            vnpay.AddRequestData("vnp_TxnRef", tick);
            // Mã tham chiếu của giao dịch tại hệ 
            //thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không được
            //trùng lặp trong ngày

            var paymentUrl = vnpay.CreateRequestUrl(config["Vnpay:BaseUrl"], config["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));

            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
			long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;

			bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, config["Vnpay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }
            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "Vnpay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
