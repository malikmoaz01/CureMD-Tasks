// ===== Abstractions =====

// Strategy Pattern Interface
public interface IPaymentStrategy
{
    void ProcessPayment(Order order);
}

// Repository Pattern Interface
public interface IOrderRepository
{
    Order GetOrder(int orderId);
}

// Logger Abstraction
public interface ILoggerService
{
    void Log(string message);
}

// ===== Domain Entity =====
public class Order
{
    public int OrderId { get; set; }
    public string PaymentType { get; set; }
    public decimal Amount { get; set; }
}

// ===== Payment Strategies =====

public class CreditCardPayment : IPaymentStrategy
{
    public void ProcessPayment(Order order)
    {
        Console.WriteLine($"Processing Credit Card payment for Order {order.OrderId}");
        // Actual credit card processing logic
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void ProcessPayment(Order order)
    {
        Console.WriteLine($"Processing PayPal payment for Order {order.OrderId}");
        // Actual PayPal processing logic
    }
}

// ===== Factory Pattern =====

public class PaymentStrategyFactory
{
    public IPaymentStrategy GetPaymentStrategy(string paymentType)
    {
        return paymentType switch
        {
            "CreditCard" => new CreditCardPayment(),
            "PayPal" => new PayPalPayment(),
            _ => throw new NotSupportedException($"Payment type {paymentType} not supported")
        };
    }
}

// ===== Repository Implementation =====

public class OrderRepository : IOrderRepository
{
    public Order GetOrder(int orderId)
    {
        // Simulating DB call
        return new Order
        {
            OrderId = orderId,
            PaymentType = orderId % 2 == 0 ? "CreditCard" : "PayPal",
            Amount = 100
        };
    }
}

// ===== Logger Implementation =====

public class ConsoleLogger : ILoggerService
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG]: {message}");
    }
}

// ===== Order Processor =====

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

// ===== Program Entry =====

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
