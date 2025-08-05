using System;
using System.Collections.Generic;

namespace ECommerceOrderSystem.Models
{
    public enum ProductCategory
    {
        Electronics,
        Clothing,
        Books,
        HomeGarden
    }

    public abstract class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ProductCategory Category { get; set; }

        public abstract string GetProductDetails();
    }

    public class Electronics : Product
    {
        public int WarrantyYears { get; set; }
        public string Brand { get; set; }

        public override string GetProductDetails()
        {
            return $"Electronics: {Name} - Brand: {Brand}, Warranty: {WarrantyYears} years, Price: ${Price}";
        }
    }

    public class Clothing : Product
    {
        public string Size { get; set; }
        public string Material { get; set; }

        public override string GetProductDetails()
        {
            return $"Clothing: {Name} - Size: {Size}, Material: {Material}, Price: ${Price}";
        }
    }

    public class Books : Product
    {
        public string ISBN { get; set; }
        public string Author { get; set; }

        public override string GetProductDetails()
        {
            return $"Book: {Name} - Author: {Author}, ISBN: {ISBN}, Price: ${Price}";
        }
    }

    public class HomeGarden : Product
    {
        public string Type { get; set; }
        public bool IsOutdoorUse { get; set; }

        public override string GetProductDetails()
        {
            return $"Home & Garden: {Name} - Type: {Type}, Outdoor Use: {IsOutdoorUse}, Price: ${Price}";
        }
    }








    public class Order
    {

        private static int _nextId = 0;

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            // Id = Guid.NewGuid().ToString();
            // Id = nextId++;
            Id = Interlocked.Increment(ref _nextId).ToString(); 
            OrderDate = DateTime.Now;
            Status = "Pending";
        }
    }







    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}