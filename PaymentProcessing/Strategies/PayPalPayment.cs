using System;
using PaymentProcessing.Abstractions;
using PaymentProcessing.Models;

namespace PaymentProcessing.Strategies
{
    public class PayPalPayment : IPaymentStrategy
    {
        public void ProcessPayment(Order order)
        {
            Console.WriteLine($"Processing PayPal payment for Order {order.OrderId}");
        }
    }
}