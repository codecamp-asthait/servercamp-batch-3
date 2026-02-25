namespace CoffeeShopSystem.Patterns;

public interface IDiscountStategy
{
    decimal ApplyDiscount(decimal total);
}

public class NoDiscountStrategy : IDiscountStategy
{
    public decimal ApplyDiscount(decimal total) => total;
}

public class HappyHourStrategy : IDiscountStategy
{
    public decimal ApplyDiscount(decimal total) => total * 0.90m;
}