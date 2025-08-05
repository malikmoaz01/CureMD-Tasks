using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceOrderSystem.Models;
using ECommerceOrderSystem.Repositories;
using ECommerceOrderSystem.Services;
using ECommerceOrderSystem.Strategies;
using ECommerceOrderSystem.Singletons;

namespace ECommerceOrderSystem
{
    class Program
    {
        private static ProductService _productService;
        private static OrderService _orderService;
        private static PaymentService _paymentService;
        private static ILogger _logger;
        private static IConfigurationManager _configManager;

        static async Task Main(string[] args)
        {
            InitializeServices();
            await SeedData();

            bool running = true;
            while (running)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ViewProducts();
                        break;
                    case "2":
                        await AddNewProduct();
                        break;
                    case "3":
                        await CreateOrder();
                        break;
                    case "4":
                        await ProcessPayment();
                        break;
                    case "5":
                        await ViewOrders();
                        break;
                    case "6":
                        await CheckInventory();
                        break;
                    case "7":
                        ViewSystemLogs();
                        break;
                    case "8":
                        ViewConfiguration();
                        break;
                    case "9":
                        running = false;
                        Console.WriteLine("Thank you for using E-Commerce Order Management System!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void InitializeServices()
        {
            var productRepository = new InMemoryProductRepository();
            var orderRepository = new InMemoryOrderRepository();

            _productService = new ProductService(productRepository);
            _orderService = new OrderService(orderRepository, productRepository);
            _paymentService = new PaymentService();
            _logger = Logger.Instance;
            _configManager = ConfigurationManager.Instance;

            _logger.LogInfo("E-Commerce Order Management System started");
        }

        private static async Task SeedData()
        {
            await _productService.CreateProduct(ProductCategory.Electronics, "Laptop", 999.99m, 10);
            await _productService.CreateProduct(ProductCategory.Electronics, "Smartphone", 599.99m, 15);
            await _productService.CreateProduct(ProductCategory.Clothing, "T-Shirt", 29.99m, 50);
            await _productService.CreateProduct(ProductCategory.Books, "Programming Book", 49.99m, 20);
            await _productService.CreateProduct(ProductCategory.HomeGarden, "Garden Tool", 39.99m, 8);
        }

        private static void ShowMenu()
        {
            Console.WriteLine("=== E-Commerce Order Management System ===");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add New Product");
            Console.WriteLine("3. Create Order");
            Console.WriteLine("4. Process Payment");
            Console.WriteLine("5. View Orders");
            Console.WriteLine("6. Check Inventory");
            Console.WriteLine("7. System Logs");
            Console.WriteLine("8. Configuration");
            Console.WriteLine("9. Exit");
            Console.Write("Please select an option: ");
        }

        private static async Task ViewProducts()
        {
            Console.WriteLine("\n=== Products ===");
            var products = await _productService.GetAllProducts();

            if (!products.Any())
            {
                Console.WriteLine("No products available.");
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}");
                Console.WriteLine(product.GetProductDetails());
                Console.WriteLine($"Stock: {product.StockQuantity}");
                Console.WriteLine("---");
            }
        }

        private static async Task AddNewProduct()
        {
            Console.WriteLine("\n=== Add New Product ===");
            
            Console.WriteLine("Select Category:");
            Console.WriteLine("1. Electronics");
            Console.WriteLine("2. Clothing");
            Console.WriteLine("3. Books");
            Console.WriteLine("4. Home & Garden");
            Console.Write("Choice: ");
            
            if (!int.TryParse(Console.ReadLine(), out int categoryChoice) || categoryChoice < 1 || categoryChoice > 4)
            {
                Console.WriteLine("Invalid category selection.");
                return;
            }

            var category = (ProductCategory)(categoryChoice - 1);

            Console.Write("Product Name: ");
            string name = Console.ReadLine();

            Console.Write("Price: $");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            Console.Write("Stock Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Invalid stock quantity.");
                return;
            }

            try
            {
                var product = await _productService.CreateProduct(category, name, price, stock);
                Console.WriteLine($"Product created successfully: {product.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
            }
        }

        private static async Task CreateOrder()
        {
            Console.WriteLine("\n=== Create Order ===");
            
            Console.Write("Customer ID: ");
            string customerId = Console.ReadLine();

            var orderItems = new List<(string productId, int quantity)>();
            
            while (true)
            {
                Console.Write("Product ID (or 'done' to finish): ");
                string productId = Console.ReadLine();
                
                if (productId.ToLower() == "done")
                    break;

                Console.Write("Quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                orderItems.Add((productId, quantity));
            }

            if (!orderItems.Any())
            {
                Console.WriteLine("No items added to order.");
                return;
            }

            try
            {
                var order = await _orderService.CreateOrder(customerId, orderItems);
                Console.WriteLine($"Order created successfully!");
                Console.WriteLine($"Order ID: {order.Id}");
                Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
                Console.WriteLine($"Items: {order.Items.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
            }
        }

        private static async Task ProcessPayment()
        {
            Console.WriteLine("\n=== Process Payment ===");
            
            Console.Write("Order ID: ");
            string orderId = Console.ReadLine();

            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            if (order.Status == "Paid")
            {
                Console.WriteLine("Order is already paid.");
                return;
            }

            Console.WriteLine($"Order Total: ${order.TotalAmount:F2}");
            Console.WriteLine("Select Payment Method:");
            Console.WriteLine("1. Credit Card");
            Console.WriteLine("2. PayPal");
            Console.WriteLine("3. Bank Transfer");
            Console.WriteLine("4. Cryptocurrency");
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out int paymentChoice))
            {
                Console.WriteLine("Invalid payment method selection.");
                return;
            }

            IPaymentStrategy paymentStrategy = null;

            switch (paymentChoice)
            {
                case 1:
                    Console.Write("Card Number (16 digits): ");
                    string cardNumber = Console.ReadLine();
                    Console.Write("Expiry Date (MM/YY): ");
                    string expiry = Console.ReadLine();
                    Console.Write("CVV (3 digits): ");
                    string cvv = Console.ReadLine();
                    paymentStrategy = new CreditCardPayment(cardNumber, expiry, cvv);
                    break;

                case 2:
                    Console.Write("PayPal Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    paymentStrategy = new PayPalPayment(email, password);
                    break;

                case 3:
                    Console.Write("Routing Number (9 digits): ");
                    string routingNumber = Console.ReadLine();
                    Console.Write("Account Number: ");
                    string accountNumber = Console.ReadLine();
                    paymentStrategy = new BankTransferPayment(routingNumber, accountNumber);
                    break;

                case 4:
                    Console.Write("Wallet Address: ");
                    string walletAddress = Console.ReadLine();
                    paymentStrategy = new CryptoPayment(walletAddress);
                    break;

                default:
                    Console.WriteLine("Invalid payment method.");
                    return;
            }

            bool success = _paymentService.ProcessPayment(order, paymentStrategy);
            
            if (success)
            {
                Console.WriteLine("Payment processed successfully!");
            }
            else
            {
                Console.WriteLine("Payment failed. Please check your payment information.");
            }
        }

        private static async Task ViewOrders()
        {
            Console.WriteLine("\n=== Orders ===");
            var orders = await _orderService.GetAllOrders();

            if (!orders.Any())
            {
                Console.WriteLine("No orders found.");
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id}");
                Console.WriteLine($"Customer ID: {order.CustomerId}");
                Console.WriteLine($"Order Date: {order.OrderDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Status: {order.Status}");
                Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
                Console.WriteLine($"Items: {order.Items.Count}");
                
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"  - {item.ProductName} x{item.Quantity} @ ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
                }
                Console.WriteLine("---");
            }
        }

        private static async Task CheckInventory()
        {
            Console.WriteLine("\n=== Inventory Check ===");
            Console.WriteLine("1. View All Products");
            Console.WriteLine("2. View Low Stock Products");
            Console.WriteLine("3. View Products by Category");
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            switch (choice)
            {
                case 1:
                    await ViewProducts();
                    break;

                case 2:
                    Console.Write("Enter stock threshold (default 5): ");
                    if (!int.TryParse(Console.ReadLine(), out int threshold))
                        threshold = 5;

                    var lowStockProducts = await _productService.GetLowStockProducts(threshold);
                    
                    if (!lowStockProducts.Any())
                    {
                        Console.WriteLine("No low stock products found.");
                        return;
                    }

                    Console.WriteLine($"Products with stock <= {threshold}:");
                    foreach (var product in lowStockProducts)
                    {
                        Console.WriteLine($"{product.Name} - Stock: {product.StockQuantity}");
                    }
                    break;

                case 3:
                    Console.WriteLine("Select Category:");
                    Console.WriteLine("1. Electronics");
                    Console.WriteLine("2. Clothing");
                    Console.WriteLine("3. Books");
                    Console.WriteLine("4. Home & Garden");
                    Console.Write("Choice: ");

                    if (!int.TryParse(Console.ReadLine(), out int categoryChoice) || categoryChoice < 1 || categoryChoice > 4)
                    {
                        Console.WriteLine("Invalid category selection.");
                        return;
                    }

                    var category = (ProductCategory)(categoryChoice - 1);
                    var categoryProducts = await _productService.GetProductsByCategory(category);

                    if (!categoryProducts.Any())
                    {
                        Console.WriteLine($"No products found in {category} category.");
                        return;
                    }

                    Console.WriteLine($"Products in {category} category:");
                    foreach (var product in categoryProducts)
                    {
                        Console.WriteLine(product.GetProductDetails());
                        Console.WriteLine($"Stock: {product.StockQuantity}");
                        Console.WriteLine("---");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static void ViewSystemLogs()
        {
            Console.WriteLine("\n=== System Logs ===");
            Console.WriteLine("Recent system activity has been logged to console.");
            Console.WriteLine("In a real application, logs would be stored in files or database.");
            
            _logger.LogInfo("System logs viewed by user");
            _logger.LogWarning("This is a sample warning message");
            _logger.LogError("This is a sample error message");
        }

        private static void ViewConfiguration()
        {
            Console.WriteLine("\n=== System Configuration ===");
            Console.WriteLine("Current Settings:");
            Console.WriteLine($"Database Connection: {_configManager.GetSetting("DatabaseConnection")}");
            Console.WriteLine($"API Key: {_configManager.GetSetting("ApiKey")}");
            Console.WriteLine($"Max Order Items: {_configManager.GetSetting("MaxOrderItems")}");
            Console.WriteLine($"Currency: {_configManager.GetSetting("Currency")}");

            Console.WriteLine("\nWould you like to update a setting? (y/n): ");
            string response = Console.ReadLine();

            if (response?.ToLower() == "y")
            {
                Console.Write("Setting Key: ");
                string key = Console.ReadLine();
                Console.Write("New Value: ");
                string value = Console.ReadLine();

                _configManager.SetSetting(key, value);
                Console.WriteLine($"Setting '{key}' updated to '{value}'");
                _logger.LogInfo($"Configuration updated: {key} = {value}");
            }
        }
    }
}