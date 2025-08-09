using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public class PaymentContext
    {
        private IPaymentStrategy paymentStrategy;

        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            paymentStrategy = strategy;
        }

        public PaymentResult ExecutePayment(decimal amount)
        {
            if (paymentStrategy == null)
            {
                return new PaymentResult
                {
                    IsSuccessful = false,
                    Message = "No payment strategy selected"
                };
            }

            return paymentStrategy.ProcessPayment(amount);
        }
    }
}