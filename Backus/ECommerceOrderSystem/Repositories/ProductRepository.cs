using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceOrderSystem.Models;

namespace ECommerceOrderSystem.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }

    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategory(ProductCategory category);
        Task<IEnumerable<Product>> GetLowStockProducts(int threshold);
    }

    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomer(string customerId);
        Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime start, DateTime end);
    }

    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products;

        public InMemoryProductRepository()
        {
            _products = new List<Product>();
        }

        public Task<Product> GetByIdAsync(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.AsEnumerable());
        }

        public Task AddAsync(Product entity)
        {
            _products.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product entity)
        {
            var existing = _products.FirstOrDefault(p => p.Id == entity.Id);
            if (existing != null)
            {
                var index = _products.IndexOf(existing);
                _products[index] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Product>> GetByCategory(ProductCategory category)
        {
            var products = _products.Where(p => p.Category == category);
            return Task.FromResult(products);
        }

        public Task<IEnumerable<Product>> GetLowStockProducts(int threshold)
        {
            var lowStockProducts = _products.Where(p => p.StockQuantity <= threshold);
            return Task.FromResult(lowStockProducts);
        }
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders;

        public InMemoryOrderRepository()
        {
            _orders = new List<Order>();
        }

        public Task<Order> GetByIdAsync(string id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            return Task.FromResult(_orders.AsEnumerable());
        }

        public Task AddAsync(Order entity)
        {
            _orders.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Order entity)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == entity.Id);
            if (existing != null)
            {
                var index = _orders.IndexOf(existing);
                _orders[index] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _orders.Remove(order);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Order>> GetOrdersByCustomer(string customerId)
        {
            var orders = _orders.Where(o => o.CustomerId == customerId);
            return Task.FromResult(orders);
        }

        public Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime start, DateTime end)
        {
            var orders = _orders.Where(o => o.OrderDate >= start && o.OrderDate <= end);
            return Task.FromResult(orders);
        }
    }
}