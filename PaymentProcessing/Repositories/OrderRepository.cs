using PaymentProcessing.Abstractions;
using PaymentProcessing.Models;

namespace PaymentProcessing.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public Order GetOrder(int orderId)
        {
            return new Order
            {
                OrderId = orderId,
                PaymentType = orderId % 2 == 0 ? "CreditCard" : "PayPal",
                Amount = 100
            };
        }
    }
}