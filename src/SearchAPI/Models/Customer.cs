using Redis.OM.Modeling;

namespace SearchAPI.Models;

[Document(StorageType = StorageType.Json)]
public class Customer
{
    [Indexed]
    public string FirstName { get; set; }

    [Indexed]
    public string LastName { get; set; }

    [Indexed]
    public string Email { get; set; }

    [Indexed(Sortable = true)]
    public int Age { get; set; }
}

