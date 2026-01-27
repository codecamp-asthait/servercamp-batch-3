var paymentMethod = PaymentFactory.GetPaymentMethod("card");
paymentMethod.Pay(500);

var receiptGenerator = ReceiptFactory.GetReceiptGenerator("email");
receiptGenerator.GenerateReceipt();

// TODO: Implement the Abstract Factory Pattern to enforce
// Card must use EmailReceiptGenerator
// Bkash must use PaperReceiptGenerator

class PaymentFactory
{
    public static Payment GetPaymentMethod(string method)
    {
        return method.ToLower() switch
        {
            "card" => new CardPayment(),
            "bkash" => new BkashPayment(),
            _ => throw new ArgumentException("Invalid payment method"),
        };
    }
}

interface Payment
{
    void Pay(decimal amount);
}

class CardPayment : Payment
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Card");
    }
}

class BkashPayment : Payment
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"💰 Paid {amount} Taka via Bkash");
    }
}

class ReceiptFactory
{
    public static ReceiptGenerator GetReceiptGenerator(string type)
    {
        return type.ToLower() switch
        {
            "paper" => new PaperReceiptGenerator(),
            "email" => new EmailReceiptGenerator(),
            _ => throw new ArgumentException("Invalid receipt type"),
        };
    }
}

interface ReceiptGenerator
{
    void GenerateReceipt();
}

class PaperReceiptGenerator : ReceiptGenerator
{
    public void GenerateReceipt()
    {
        // Implementation for generating paper receipt
    }
}

class EmailReceiptGenerator : ReceiptGenerator
{
    public void GenerateReceipt()
    {
        // Implementation for generating email receipt
    }
}
