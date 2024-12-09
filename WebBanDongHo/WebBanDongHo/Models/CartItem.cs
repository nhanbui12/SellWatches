namespace WebBanDongHo.Models
{
    public class CartItem
    {
        public int product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_image1 {  get; set; }
        public decimal product_price { get; set; }
        public int quantity {  get; set; }
    }
}
