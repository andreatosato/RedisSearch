using Bogus;
using NRediSearch;
using NSearchAPI.Models;

namespace SearchAPI
{
    public class BootstrapData
    {
        private readonly Client client;

        public BootstrapData(Client client)
        {
            this.client = client;
        }

        public async Task BootstrapAsync(int generate)
        {
            var data = GenerateCustomer(generate);
            foreach (var d in data)
            {
                Console.WriteLine(d);
                await client.AddDocumentAsync(d.GetDocument());
            }           
        }

        private List<Customer> GenerateCustomer(int counts)
        {
            List<Customer> result = new List<Customer>(counts);
            var faker = new Faker<Customer>()
                .RuleFor(o => o.FirstName, f => f.Person.FirstName)
                .RuleFor(o => o.LastName, f => f.Person.LastName)
                .RuleFor(o => o.Email, f => f.Person.Email)
                .RuleFor(o => o.Age, f => f.PickRandom(new[] {20,30,40}));
            for (int i = 0; i < counts; i++)
            {
                result.Add(faker.Generate());
            }
            return result;
        }
    }
}
