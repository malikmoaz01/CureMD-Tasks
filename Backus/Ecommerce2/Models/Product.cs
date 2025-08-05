using System;

namespace ECommerce2.Models
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
        public ProductCategory Category { get; set; }
        public int Stock { get; set; }

        public abstract string GetProductDetails();
    }

    public class Electronics : Product
    {
        public int WarrantyMonths { get; set; }
        public string Brand { get; set; }

        public override string GetProductDetails()
        {
            return $"Electronics: {Name} - ${Price} | Brand: {Brand} | Warranty: {WarrantyMonths} months | Stock: {Stock}";
        }
    }

    public class Clothing : Product
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }

        public override string GetProductDetails()
        {
            return $"Clothing: {Name} - ${Price} | Size: {Size} | Color: {Color} | Material: {Material} | Stock: {Stock}";
        }
    }

    public class Books : Product
    {
        public string ISBN { get; set; }
        public string Author { get; set; }
        public int Pages { get; set; }

        public override string GetProductDetails()
        {
            return $"Book: {Name} - ${Price} | Author: {Author} | ISBN: {ISBN} | Pages: {Pages} | Stock: {Stock}";
        }
    }

    public class HomeGarden : Product
    {
        public string Type { get; set; }
        public string Dimensions { get; set; }
        public bool IsOutdoor { get; set; }

        public override string GetProductDetails()
        {
            return $"Home & Garden: {Name} - ${Price} | Type: {Type} | Dimensions: {Dimensions} | Outdoor: {IsOutdoor} | Stock: {Stock}";
        }
    }
}