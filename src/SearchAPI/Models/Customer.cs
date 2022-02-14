using Redis.OM.Modeling;

namespace SearchAPI.Models;

[Document(StorageType = StorageType.Json)]
public class Customer
{
    [Indexed]
    public string FirstName { get; set; } = null!;

    [Indexed]
    public string LastName { get; set; } = null!;

    [Indexed]
    public string Email { get; set; } = null!;

    [Indexed(Sortable = true)]
    public int Age { get; set; }
}

