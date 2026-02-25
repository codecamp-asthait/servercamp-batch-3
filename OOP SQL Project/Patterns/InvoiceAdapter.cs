namespace CoffeeShopSystem.Patterns;

public interface IInvoiceService
{
    string GenerateInvoice(decimal amount, string customer);
}

public class LegacyInvoiceSystem
{
    public string CreateLegacyInvoice(decimal amount, string customer)
    {
        return $"Legacy Invoice generated for {customer}: {amount:0.00} BDT";
    }
}

public class InvoiceAdapter : IInvoiceService
{
    private readonly LegacyInvoiceSystem _legacySystem;

    public InvoiceAdapter(LegacyInvoiceSystem legacySystem)
    {
        _legacySystem = legacySystem;
    }

    public string GenerateInvoice(decimal amount, string customer)
    {
        // transformations
        return _legacySystem.CreateLegacyInvoice(amount, customer);
    }
}