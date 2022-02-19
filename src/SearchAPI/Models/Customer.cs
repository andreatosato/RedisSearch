using Redis.OM.Modeling;

namespace SearchAPI.Models;

[Document]
public class Customer
{
    //[RedisIdField]
    //public Ulid Id { get; set; }

    [Indexed(Sortable = true)]
    public string FirstName { get; set; } = null!;

    [Indexed(Sortable = true)]
    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    [Indexed(Aggregatable = true)]
    public int Age { get; set; }

    public override string ToString()
    {
        return $"{FirstName}, {LastName}, {Email}, {Age}";
    }
}

public class AggregationCustomer
{
    public string LastName { get; set; } = null!;
    public int Cout { get; set; }
}

public class AggregationAgeCustomer
{
    public int Age { get; set; }
    public int Cout { get; set; }
}

