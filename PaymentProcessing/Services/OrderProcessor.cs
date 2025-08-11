using PaymentProcessing.Abstractions;
using PaymentProcessing.Factories;

namespace PaymentProcessing.Services
{
    public class OrderProcessor
    {
        private readonly IOrderRepository _orderRepository;
        private readonly PaymentStrategyFactory _paymentFactory;
        private readonly ILoggerService _logger;

        public OrderProcessor(IOrderRepository orderRepository,
                              PaymentStrategyFactory paymentFactory,
                              ILoggerService logger)
        {
            _orderRepository = orderRepository;
            _paymentFactory = paymentFactory;
            _logger = logger;
        }

        public void ProcessOrder(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            var paymentStrategy = _paymentFactory.GetPaymentStrategy(order.PaymentType);
            paymentStrategy.ProcessPayment(order);
            _logger.Log($"Order {orderId} processed successfully");
        }
    }
}