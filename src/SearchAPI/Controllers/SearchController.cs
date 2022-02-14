using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Aggregation;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using SearchAPI.Models;

namespace SearchAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly RedisConnectionProvider redisConnectionProvider = null!;
    private IRedisCollection<Customer> collection => redisConnectionProvider.RedisCollection<Customer>();
    private IRedisConnection connection => redisConnectionProvider.Connection;
    private RedisAggregationSet<Customer> aggregation => redisConnectionProvider.AggregationSet<Customer>();

    public SearchController(RedisConnectionProvider redisConnectionProvider)
    {
        this.redisConnectionProvider = redisConnectionProvider;
        //connection.DropIndex(typeof(Customer));
        connection.CreateIndex(typeof(Customer));
    }

    [HttpGet]
    public IActionResult GetAsync(string firstName)
    {
        var result = collection.Where(t => t.FirstName == firstName).ToList();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Customer customer)
    {
        await collection.InsertAsync(customer);
        return Created($"search?firstName={customer.FirstName}", customer);
    }
}

