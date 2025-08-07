using System;
using System.Collections.Generic;
using ECommerce2.Models;
using ECommerce2.Factories;
using ECommerce2.Singletons;
using ECommerce2.Strategies;
using ECommerce2.Repositories;

namespace ECommerce2.Services
{
    public class ECommerceSystem
    {
        private IProductRepository productRepository;
        private IOrderRepository orderRepository;
        private PaymentContext paymentContext;
        private ILogger logger;
        private IConfigurationManager config;
        private int nextOrderId;

        public ECommerceSystem()
        {
            productRepository = new ProductRepository();
            orderRepository = new OrderRepository();
            paymentContext = new PaymentContext();
            logger = Logger.Instance;
            config = ConfigurationManager.Instance;
            nextOrderId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            productRepository.AddAsync(ProductFactory.CreateProduct(ProductCategory.Electronics, "Smartphone", 599.99m, 10));
            productRepository.AddAsync(ProductFactory.CreateProduct(ProductCategory.Clothing, "T-Shirt", 29.99m, 25));
            productRepository.AddAsync(ProductFactory.CreateProduct(ProductCategory.Books, "Programming Guide", 49.99m, 15));
            productRepository.AddAsync(ProductFactory.CreateProduct(ProductCategory.HomeGarden, "Garden Tool", 19.99m, 8));
        }

        public void ViewProducts()
        {
            Console.WriteLine("\n=== Product Catalog ===");
            var products = productRepository.GetAllAsync().Result;
            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i].GetProductDetails()}");
            }
        }

        public void AddNewProduct()
        {
            Console.WriteLine("\n=== Add New Product ===");
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();

            Console.Write("Enter price: $");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
            {
                Console.Write("Invalid price. Enter a valid price: $");
            }

            Console.Write("Enter stock quantity: ");
            int stock;
            while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
            {
                Console.Write("Invalid quantity. Enter a valid stock quantity: ");
            }

            Console.WriteLine("Select category:");
            Console.WriteLine("1. Electronics");
            Console.WriteLine("2. Clothing");
            Console.WriteLine("3. Books");
            Console.WriteLine("4. Home & Garden");
            Console.Write("Enter choice (1-4): ");

            int categoryChoice;
            while (!int.TryParse(Console.ReadLine(), out categoryChoice) || categoryChoice < 1 || categoryChoice > 4)
            {
                Console.Write("Invalid choice. Enter 1-4: ");
            }

            ProductCategory category = (ProductCategory)(categoryChoice - 1);

            try
            {
                Product newProduct = ProductFactory.CreateProduct(category, name, price, stock);
                productRepository.AddAsync(newProduct);
                logger.LogInfo($"New product added: {newProduct.Name}");
                Console.WriteLine("Product added successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to add product: {ex.Message}");
                Console.WriteLine("Failed to add product.");
            }
        }

        public void CreateOrder()
        {
            Console.WriteLine("\n=== Create New Order ===");
            Console.Write("Enter customer ID: ");
            string customerId = Console.ReadLine();

            Order order = new Order
            {
                Id = nextOrderId.ToString(),
                CustomerId = customerId
            };
            nextOrderId++;

            bool addingItems = true;
            while (addingItems)
            {
                ViewProducts();
                Console.Write("Select product number (or 0 to finish): ");
                int productChoice;
                if (!int.TryParse(Console.ReadLine(), out productChoice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (productChoice == 0)
                {
                    addingItems = false;
                    continue;
                }

                var products = productRepository.GetAllAsync().Result;
                if (productChoice < 1 || productChoice > products.Count)
                {
                    Console.WriteLine("Invalid product selection.");
                    continue;
                }

                Product selectedProduct = products[productChoice - 1];
                Console.Write($"Enter quantity for {selectedProduct.Name}: ");
                int quantity;
                if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                if (quantity > selectedProduct.Stock)
                {
                    Console.WriteLine($"Insufficient stock. Available: {selectedProduct.Stock}");
                    continue;
                }

                OrderItem orderItem = new OrderItem
                {
                    Product = selectedProduct,
                    Quantity = quantity
                };

                order.Items.Add(orderItem);
                selectedProduct.Stock -= quantity;
                productRepository.UpdateAsync(selectedProduct);
                Console.WriteLine($"Added {quantity} x {selectedProduct.Name} to order.");
            }

            if (order.Items.Count > 0)
            {
                orderRepository.AddAsync(order);
                logger.LogInfo($"Order created: {order.Id} for customer {order.CustomerId}");
                Console.WriteLine($"Order created successfully! Order ID: {order.Id}, Total: ${order.TotalAmount:F2}");
            }
            else
            {
                Console.WriteLine("No items added to order.");
            }
        }

        public void ProcessPayment()
        {
            Console.WriteLine("\n=== Process Payment ===");
            var orders = orderRepository.GetAllAsync().Result;
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders available for payment.");
                return;
            }

            Console.WriteLine("Available orders:");
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Status == "Pending")
                {
                    Console.WriteLine($"{i + 1}. Order ID: {orders[i].Id}, Amount: ${orders[i].TotalAmount:F2}");
                }
            }

            Console.Write("Select order number: ");
            int orderChoice;
            if (!int.TryParse(Console.ReadLine(), out orderChoice) || orderChoice < 1 || orderChoice > orders.Count)
            {
                Console.WriteLine("Invalid order selection.");
                return;
            }

            Order selectedOrder = orders[orderChoice - 1];
            if (selectedOrder.Status != "Pending")
            {
                Console.WriteLine("Order has already been processed.");
                return;
            }

            Console.WriteLine("Select payment method:");
            Console.WriteLine("1. Credit Card");
            Console.WriteLine("2. PayPal");
            Console.WriteLine("3. Bank Transfer");
            Console.WriteLine("4. Cryptocurrency");
            Console.Write("Enter choice (1-4): ");

            int paymentChoice;
            if (!int.TryParse(Console.ReadLine(), out paymentChoice) || paymentChoice < 1 || paymentChoice > 4)
            {
                Console.WriteLine("Invalid payment method selection.");
                return;
            }

            IPaymentStrategy paymentStrategy = null;

            switch (paymentChoice)
            {
                case 1:
                    Console.Write("Enter card number (16 digits): ");
                    string cardNumber = Console.ReadLine();
                    Console.Write("Enter expiry date (MM/YY): ");
                    string expiry = Console.ReadLine();
                    Console.Write("Enter CVV (3 digits): ");
                    string cvv = Console.ReadLine();
                    Console.Write("Enter cardholder name: ");
                    string holderName = Console.ReadLine();
                    paymentStrategy = new CreditCardPayment(cardNumber, expiry, cvv, holderName);
                    selectedOrder.PaymentMethod = "Credit Card";
                    break;
                case 2:
                    Console.Write("Enter PayPal email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter PayPal password: ");
                    string password = Console.ReadLine();
                    paymentStrategy = new PayPalPayment(email, password);
                    selectedOrder.PaymentMethod = "PayPal";
                    break;
                case 3:
                    Console.Write("Enter routing number (9 digits): ");
                    string routing = Console.ReadLine();
                    Console.Write("Enter account number: ");
                    string account = Console.ReadLine();
                    Console.Write("Enter bank name: ");
                    string bankName = Console.ReadLine();
                    paymentStrategy = new BankTransferPayment(routing, account, bankName);
                    selectedOrder.PaymentMethod = "Bank Transfer";
                    break;
                case 4:
                    Console.Write("Enter wallet address: ");
                    string walletAddress = Console.ReadLine();
                    Console.Write("Enter cryptocurrency type: ");
                    string cryptoType = Console.ReadLine();
                    paymentStrategy = new CryptoPayment(walletAddress, cryptoType);
                    selectedOrder.PaymentMethod = "Cryptocurrency";
                    break;
            }

            paymentContext.SetPaymentStrategy(paymentStrategy);
            PaymentResult result = paymentContext.ExecutePayment(selectedOrder.TotalAmount);

            if (result.IsSuccessful)
            {
                selectedOrder.Status = "Paid";
                orderRepository.UpdateAsync(selectedOrder);
                logger.LogInfo($"Payment processed successfully for order {selectedOrder.Id}: {result.TransactionId}");
                Console.WriteLine($"Payment successful! Transaction ID: {result.TransactionId}");
            }
            else
            {
                logger.LogError($"Payment failed for order {selectedOrder.Id}: {result.Message}");
                Console.WriteLine($"Payment failed: {result.Message}");
            }
        }

        public void ViewOrders()
        {
            Console.WriteLine("\n=== Order History ===");
            var orders = orderRepository.GetAllAsync().Result;
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            for (int i = 0; i < orders.Count; i++)
            {
                Order order = orders[i];
                Console.WriteLine($"Order ID: {order.Id}");
                Console.WriteLine($"Customer: {order.CustomerId}");
                Console.WriteLine($"Date: {order.OrderDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Status: {order.Status}");
                Console.WriteLine($"Payment Method: {order.PaymentMethod ?? "Not Set"}");
                Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
                Console.WriteLine("Items:");
                for (int j = 0; j < order.Items.Count; j++)
                {
                    OrderItem item = order.Items[j];
                    Console.WriteLine($"  - {item.Product.Name} x{item.Quantity} = ${item.TotalPrice:F2}");
                }
                Console.WriteLine();
            }
        }

        public void CheckInventory()
        {
            Console.WriteLine("\n=== Inventory Status ===");
            var products = productRepository.GetAllAsync().Result;
            if (products.Count == 0)
            {
                Console.WriteLine("No products in inventory.");
                return;
            }

            Console.WriteLine("\n--- Regular Inventory ---");
            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                Console.WriteLine($"{product.Name} - Stock: {product.Stock}");
            }

            var lowStockProducts = productRepository.GetLowStockProducts(5);
            if (lowStockProducts.Count > 0)
            {
                Console.WriteLine("\n--- LOW STOCK ALERT ---");
                for (int i = 0; i < lowStockProducts.Count; i++)
                {
                    Console.WriteLine($"{lowStockProducts[i].Name} - Stock: {lowStockProducts[i].Stock} (LOW STOCK)");
                }
            }
        }

        public void ViewSystemLogs()
        {
            Console.WriteLine("\n=== System Logs ===");
            Console.WriteLine("Recent system activity has been logged to console.");
            logger.LogInfo("System logs accessed by user");
        }

        public void ManageConfiguration()
        {
            Console.WriteLine("\n=== Configuration Management ===");
            Console.WriteLine("1. View Settings");
            Console.WriteLine("2. Update Setting");
            Console.Write("Enter choice (1-2): ");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("\nCurrent Settings:");
                    Console.WriteLine($"Database Connection: {config.GetSetting("DatabaseConnection")}");
                    Console.WriteLine($"API Key: {config.GetSetting("APIKey")}");
                    Console.WriteLine($"Max Order Items: {config.GetSetting("MaxOrderItems")}");
                    Console.WriteLine($"Tax Rate: {config.GetSetting("TaxRate")}");
                    break;
                case 2:
                    Console.Write("Enter setting key: ");
                    string key = Console.ReadLine();
                    Console.Write("Enter new value: ");
                    string value = Console.ReadLine();
                    config.SetSetting(key, value);
                    logger.LogInfo($"Configuration updated: {key} = {value}");
                    Console.WriteLine("Setting updated successfully!");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        public void ShowMenu()
        {
            Console.WriteLine("\n=== E-Commerce Order Management System ===");
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

        public void Run()
        {
            logger.LogInfo("E-Commerce system started");

            while (true)
            {
                ShowMenu();
                string input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ViewProducts();
                        break;
                    case 2:
                        AddNewProduct();
                        break;
                    case 3:
                        CreateOrder();
                        break;
                    case 4:
                        ProcessPayment();
                        break;
                    case 5:
                        ViewOrders();
                        break;
                    case 6:
                        CheckInventory();
                        break;
                    case 7:
                        ViewSystemLogs();
                        break;
                    case 8:
                        ManageConfiguration();
                        break;
                    case 9:
                        logger.LogInfo("E-Commerce system shutdown");
                        Console.WriteLine("Thank you for using E-Commerce Order Management System!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please select a number between 1 and 9.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}