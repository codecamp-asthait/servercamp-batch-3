using CoffeeShopSystem.Models;
using CoffeeShopSystem.Patterns;
using Microsoft.Data.SqlClient;

namespace CoffeeShopSystem;

class Program
{
    static void Main(string[] args)
    {
        // Factory
        ICoffee myCoffee = CoffeeFactory.CreateCoffee("lattee");

        // Decorator
        myCoffee = new MilkDecorator(myCoffee);
        myCoffee = new SugarDecorator(myCoffee);

        Console.WriteLine($"Order: {myCoffee.GetDescription()}");
        Console.WriteLine($"Price: {myCoffee.GetCost()}");

        // Strategy
        IDiscountStategy discount = new HappyHourStrategy();
        decimal discountedPrice = discount.ApplyDiscount(myCoffee.GetCost());
        Console.WriteLine($"Discounted Price: {discountedPrice:0.00} Taka");

        // Adapter
        IInvoiceService invoiceService = new InvoiceAdapter(new LegacyInvoiceSystem());
        Console.WriteLine(invoiceService.GenerateInvoice(discountedPrice, "John Doe"));

        try
        {
            using var connection = SqlDbConnection.instance.CreateConnection();
            connection.Open();

            // INSERT
            string insertQuery = "INSERT INTO Orders (CustomerId, OrderDate, TotalAmount) OUTPUT INSERTED.Id VALUES (@cid, @date, @total)";
            using var insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@cid", 1);
            insertCommand.Parameters.AddWithValue("@date", DateTime.UtcNow);
            insertCommand.Parameters.AddWithValue("@total", discountedPrice);

            int newOrderId = (int)insertCommand.ExecuteScalar();
            Console.WriteLine($"Order {newOrderId} Inserted");

            // UPDate
            string updateQuery = "UPDATE Orders SET TotalAmount = TotalAmount + 10 WHERE Id = @id";
            using var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@id", 1);
            updateCommand.ExecuteNonQuery();
            Console.WriteLine($"Order {newOrderId} updated");

            // GET
            string selectQuery = "SELECT Id, CustomerId, TotalAmount, OrderDate From Orders";
            using var selectCommand = new SqlCommand(selectQuery, connection);
            using var reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"OrderId: {reader["Id"]} | Total: {reader["TotalAmount"]} Taka | Date: {reader["OrderDate"]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database operation failed: {ex.Message}");
        }
    }
}