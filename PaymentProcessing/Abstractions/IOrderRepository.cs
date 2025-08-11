namespace PaymentProcessing.Abstractions
{
    public interface IOrderRepository
    {
        Order GetOrder(int orderId);
    }
}