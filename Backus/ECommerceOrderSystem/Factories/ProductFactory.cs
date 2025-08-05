using System;
using ECommerceOrderSystem.Models;

namespace ECommerceOrderSystem.Factories
{
    public class ProductFactory
    {
        public static Product CreateProduct(ProductCategory category, string id, string name, decimal price, int stock)
        {
            switch (category)
            {
                case ProductCategory.Electronics:
                    return new Electronics
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        StockQuantity = stock,
                        Category = category,
                        WarrantyYears = 2,
                        Brand = "Default Brand"
                    };

                case ProductCategory.Clothing:
                    return new Clothing
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        StockQuantity = stock,
                        Category = category,
                        Size = "M",
                        Material = "Cotton"
                    };

                case ProductCategory.Books:
                    return new Books
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        StockQuantity = stock,
                        Category = category,
                        ISBN = "123",
                        Author = "Unknown Author"
                    };

                case ProductCategory.HomeGarden:
                    return new HomeGarden
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        StockQuantity = stock,
                        Category = category,
                        Type = "General",
                        IsOutdoorUse = true
                    };

                default:
                    throw new ArgumentException($"Invalid product category: {category}");
            }
        }
    }
}