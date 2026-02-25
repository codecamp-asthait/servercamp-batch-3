using CoffeeShopSystem.Models;

namespace CoffeeShopSystem.Patterns;

public static class CoffeeFactory
{
    public static ICoffee CreateCoffee(string type)
    {
        return type.ToLower() switch
        {
            "espresso" => new Espresso(),
            "lattee" => new Lattee(),
            _ => new RegularCoffee()
        };
    }
}