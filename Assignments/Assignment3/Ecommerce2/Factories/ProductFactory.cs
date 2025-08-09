using System;
using ECommerce2.Models;

namespace ECommerce2.Factories
{
    public static class ProductFactory
    {
        private static int nextId = 1;

        public static Product CreateProduct(ProductCategory category, string name, decimal price, int stock)
        {
            Product product = null;
            string id = nextId.ToString();
            nextId++;

            switch (category)
            {
                case ProductCategory.Electronics:
                    product = new Electronics
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        Category = category,
                        Stock = stock,
                        WarrantyMonths = 12,
                        Brand = "Default Brand"
                    };
                    break;
                case ProductCategory.Clothing:
                    product = new Clothing
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        Category = category,
                        Stock = stock,
                        Size = "M",
                        Color = "Blue",
                        Material = "Cotton"
                    };
                    break;
                case ProductCategory.Books:
                    product = new Books
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        Category = category,
                        Stock = stock,
                        ISBN = "978-0000000000",
                        Author = "Unknown Author",
                        Pages = 200
                    };
                    break;
                case ProductCategory.HomeGarden:
                    product = new HomeGarden
                    {
                        Id = id,
                        Name = name,
                        Price = price,
                        Category = category,
                        Stock = stock,
                        Type = "Decoration",
                        Dimensions = "10x10x10",
                        IsOutdoor = false
                    };
                    break;
                default:
                    throw new ArgumentException("Invalid product category");
            }

            return product;
        }
    }
}