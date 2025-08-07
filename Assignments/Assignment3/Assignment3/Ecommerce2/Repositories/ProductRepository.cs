using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce2.Models;

namespace ECommerce2.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }

    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetByCategory(ProductCategory category);
        List<Product> GetLowStockProducts(int threshold);
    }

    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrdersByCustomer(string customerId);
        List<Order> GetOrdersByDateRange(DateTime start, DateTime end);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        protected List<T> entities;

        public InMemoryRepository()
        {
            entities = new List<T>();
        }

        public virtual Task<T> GetByIdAsync(string id)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var idProperty = entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var entityId = idProperty.GetValue(entity)?.ToString();
                    if (entityId == id)
                    {
                        return Task.FromResult(entity);
                    }
                }
            }
            return Task.FromResult<T>(null);
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            var result = new List<T>(entities);
            return Task.FromResult(result);
        }

        public virtual Task AddAsync(T entity)
        {
            entities.Add(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var entityId = idProperty.GetValue(entity)?.ToString();

                for (int i = 0; i < entities.Count; i++)
                {
                    var existingEntity = entities[i];
                    var existingIdProperty = existingEntity.GetType().GetProperty("Id");
                    if (existingIdProperty != null)
                    {
                        var existingEntityId = existingIdProperty.GetValue(existingEntity)?.ToString();
                        if (existingEntityId == entityId)
                        {
                            entities[i] = entity;
                            break;
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(string id)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                var entity = entities[i];
                var idProperty = entity.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var entityId = idProperty.GetValue(entity)?.ToString();
                    if (entityId == id)
                    {
                        entities.RemoveAt(i);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }

    public class ProductRepository : InMemoryRepository<Product>, IProductRepository
    {
        public Task<List<Product>> GetByCategory(ProductCategory category)
        {
            var result = new List<Product>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Category == category)
                {
                    result.Add(entities[i]);
                }
            }
            return Task.FromResult(result);
        }

        public List<Product> GetLowStockProducts(int threshold)
        {
            var result = new List<Product>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Stock <= threshold)
                {
                    result.Add(entities[i]);
                }
            }
            return result;
        }
    }

    public class OrderRepository : InMemoryRepository<Order>, IOrderRepository
    {
        public List<Order> GetOrdersByCustomer(string customerId)
        {
            var result = new List<Order>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].CustomerId == customerId)
                {
                    result.Add(entities[i]);
                }
            }
            return result;
        }

        public List<Order> GetOrdersByDateRange(DateTime start, DateTime end)
        {
            var result = new List<Order>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].OrderDate >= start && entities[i].OrderDate <= end)
                {
                    result.Add(entities[i]);
                }
            }
            return result;
        }
    }
}
