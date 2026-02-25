namespace CoffeeShopSystem.Models;

public class Espresso : ICoffee
{
    public string GetDescription() => "Expresso";
    public decimal GetCost() => 40.00m;
}

public class Lattee : ICoffee
{
    public string GetDescription() => "Lattee";
    public decimal GetCost() => 60.00m;
}

public class RegularCoffee : ICoffee
{
    public string GetDescription() => "Regular";
    public decimal GetCost() => 30.00m;
}