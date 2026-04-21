public class UserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserRequestWithAddress
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}

public class UserRequestWithOrder
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public List<OrderRequest> Orders { get; set; }
}

public class OrderRequest
{
    public decimal Total { get; set; }
}

