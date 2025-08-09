using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public interface IPaymentStrategy
    {
        PaymentResult ProcessPayment(decimal amount);
        bool ValidatePaymentDetails();
    }
}