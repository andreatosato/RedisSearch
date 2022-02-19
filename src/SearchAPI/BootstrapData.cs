using Bogus;
using Redis.OM;
using SearchAPI.Models;

namespace SearchAPI
{
    public class BootstrapData
    {
        private readonly RedisConnectionProvider redisConnectionProvider;

        public BootstrapData(RedisConnectionProvider redisConnectionProvider)
        {
            this.redisConnectionProvider = redisConnectionProvider;
        }

        public async Task BootstrapAsync(int generate)
        {
            var connection = redisConnectionProvider.Connection;
            connection.CreateIndex(typeof(Customer));
            connection.DropIndex(typeof(Customer));
            connection.CreateIndex(typeof(Customer));
            var customerCollection = redisConnectionProvider.RedisCollection<Customer>();
            var data = GenerateCustomer(generate);
            foreach (var d in data)
            {
                Console.WriteLine(d);
                await customerCollection.InsertAsync(d);
            }           
        }

        private List<Customer> GenerateCustomer(int counts)
        {
            List<Customer> result = new List<Customer>(counts);
            var faker = new Faker<Customer>()
                .RuleFor(o => o.FirstName, f => f.Person.FirstName)
                .RuleFor(o => o.LastName, f => f.Person.LastName)
                .RuleFor(o => o.Email, f => f.Person.Email)
                .RuleFor(o => o.Age, f => (int)Math.Floor(Math.Round(DateTime.Now.Subtract(f.Person.DateOfBirth).TotalDays / 365)));
            for (int i = 0; i < counts; i++)
            {
                result.Add(faker.Generate());
            }
            return result;
        }
    }
}
