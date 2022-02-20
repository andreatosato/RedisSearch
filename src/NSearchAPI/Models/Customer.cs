using NRediSearch;
using StackExchange.Redis;

namespace NSearchAPI.Models;

public class Customer
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Age { get; set; }

    public override string ToString()
    {
        return $"{FirstName}, {LastName}, {Email}, {Age}";
    }

    public static Schema Schema()
    {
        var schema = new Schema();
        schema.AddSortableTextField(nameof(FirstName));
        schema.AddSortableTextField(nameof(LastName));
        schema.AddSortableTextField(nameof(Email));
        schema.AddNumericField(nameof(Age));
        return schema;
    }

    public static Customer FromDocument(IEnumerable<KeyValuePair<string, RedisValue>> properties)
    {
        return new Customer
        {
            Age = (int)properties.Where(t => t.Key == nameof(Age)).FirstOrDefault().Value,
            FirstName = properties.Where(t => t.Key == nameof(FirstName)).FirstOrDefault().Value,
            LastName = properties.Where(t => t.Key == nameof(LastName)).FirstOrDefault().Value,
            Email = properties.Where(t => t.Key == nameof(Email)).FirstOrDefault().Value,
        };
    }
    public Document GetDocument()
    {
        return new Document(Guid.NewGuid().ToString(), new Dictionary<string, RedisValue>()
        {
            { "FirstName", FirstName },
            { "LastName", LastName },
            { "Email", Email },
            { "Age", Age }
        });
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
    public int Count { get; set; }
}

