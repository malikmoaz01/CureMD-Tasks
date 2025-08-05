using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceOrderSystem.Models;
using ECommerceOrderSystem.Repositories;
using ECommerceOrderSystem.Strategies;
using ECommerceOrderSystem.Singletons;
using ECommerceOrderSystem.Factories;

namespace ECommerceOrderSystem.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _logger = Logger.Instance;
        }

        public async Task<Product> CreateProduct(ProductCategory category, string name, decimal price, int stock)
        {
            try
            {
                var productId = Guid.NewGuid().ToString();
                var product = ProductFactory.CreateProduct(category, productId, name, price, stock);
                await _productRepository.AddAsync(product);
                _logger.LogInfo($"Product created: {product.Name}");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating product: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(ProductCategory category)
        {
            return await _productRepository.GetByCategory(category);
        }

        public async Task<IEnumerable<Product>> GetLowStockProducts(int threshold = 5)
        {
            return await _productRepository.GetLowStockProducts(threshold);
        }

        public async Task<Product> GetProductById(string id)
        {
            return await _productRepository.GetByIdAsync(id);
        }
    }

    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = Logger.Instance;
        }

        public async Task<Order> CreateOrder(string customerId, List<(string productId, int quantity)> orderItems)
        {
            try
            {
                var order = new Order
                {
                    CustomerId = customerId
                };

                decimal totalAmount = 0;

                foreach (var item in orderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.productId);
                    if (product == null)
                    {
                        _logger.LogWarning($"Product not found: {item.productId}");
                        continue;
                    }

                    if (product.StockQuantity < item.quantity)
                    {
                        _logger.LogWarning($"Insufficient stock for product: {product.Name}");
                        continue;
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.quantity,
                        UnitPrice = product.Price
                    };

                    order.Items.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;

                    product.StockQuantity -= item.quantity;
                    await _productRepository.UpdateAsync(product);
                }

                order.TotalAmount = totalAmount;
                await _orderRepository.AddAsync(order);
                _logger.LogInfo($"Order created: {order.Id} for customer: {customerId}");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomer(string customerId)
        {
            return await _orderRepository.GetOrdersByCustomer(customerId);
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }
    }

    public class PaymentService
    {
        private readonly PaymentContext _paymentContext;
        private readonly ILogger _logger;

        public PaymentService()
        {
            _paymentContext = new PaymentContext();
            _logger = Logger.Instance;
        }

        public bool ProcessPayment(Order order, IPaymentStrategy paymentStrategy)
        {
            try
            {
                _paymentContext.SetPaymentStrategy(paymentStrategy);
                bool success = _paymentContext.ProcessPayment(order.TotalAmount);

                if (success)
                {
                    order.Status = "Paid";
                    _logger.LogInfo($"Payment successful for order: {order.Id} using {paymentStrategy.GetPaymentMethod()}");
                }
                else
                {
                    order.Status = "Payment Failed";
                    _logger.LogError($"Payment failed for order: {order.Id}");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Payment processing error: {ex.Message}");
                return false;
            }
        }
    }
}