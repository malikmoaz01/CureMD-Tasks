using PaymentProcessing.Factories;
using PaymentProcessing.Logging;
using PaymentProcessing.Repositories;
using PaymentProcessing.Services;

namespace PaymentProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderRepository = new OrderRepository();
            var paymentFactory = new PaymentStrategyFactory();
            var logger = new ConsoleLogger();
            
            var processor = new OrderProcessor(orderRepository, paymentFactory, logger);
            
            processor.ProcessOrder(1);
            processor.ProcessOrder(2);
        }
    }
}