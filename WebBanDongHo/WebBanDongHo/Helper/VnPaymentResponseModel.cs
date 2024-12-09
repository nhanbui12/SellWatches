namespace WebBanDongHo.Helper
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }
    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        public string Name { get; set; }

        public string OrderDesciption { get; set; }

        //public DateTime TimeCheckout { get; set; }

        public double TotalMoney { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
