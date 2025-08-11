namespace PaymentProcessing.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
    }
}