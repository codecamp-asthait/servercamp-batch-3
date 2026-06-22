using System.Diagnostics.Metrics;

namespace Dukaan.Application.Observability;

public static class DukaanMetrics
{
    private static readonly Meter Meter = new("Dukaan.Application");

    public static Counter<long> CartItemsAdded { get; } =
        Meter.CreateCounter<long>("dukaan.cart_items_added", unit: "{item}",
            description: "Number of items added to shopping carts");

    public static Counter<long> CartItemsAddedValue { get; } =
        Meter.CreateCounter<long>("dukaan.cart_items_added_value", unit: "{currency}",
            description: "Total monetary value of items added to carts");

    public static Counter<long> CartItemsRemoved { get; } =
        Meter.CreateCounter<long>("dukaan.cart_items_removed", unit: "{item}",
            description: "Number of items removed from shopping carts");

    public static Counter<long> CustomerRegistrations { get; } =
        Meter.CreateCounter<long>("dukaan.customer_registrations", unit: "{registration}",
            description: "Number of customer registrations completed");

    public static Counter<long> MerchantRegistrations { get; } =
        Meter.CreateCounter<long>("dukaan.merchant_registrations", unit: "{registration}",
            description: "Number of merchant registrations completed");

    public static Counter<long> AuthLogins { get; } =
        Meter.CreateCounter<long>("dukaan.auth_logins", unit: "{login}",
            description: "Number of successful authentication logins");

    public static Counter<long> AuthFailures { get; } =
        Meter.CreateCounter<long>("dukaan.auth_failures", unit: "{failure}",
            description: "Number of failed authentication attempts");

    public static Counter<long> OrdersPlaced { get; } =
        Meter.CreateCounter<long>("dukaan.orders_placed", unit: "{order}",
            description: "Number of orders placed");

    public static Histogram<double> OrderValue { get; } =
        Meter.CreateHistogram<double>("dukaan.order_value", unit: "{currency}",
            description: "Distribution of order total values");

    public static KeyValuePair<string, object?> Tag(string key, object? value) =>
        new(key, value);
}
