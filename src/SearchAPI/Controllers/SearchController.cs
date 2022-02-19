using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Aggregation;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;
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
        //connection.CreateIndex(typeof(Customer));
    }

    [HttpGet]
    public IActionResult GetAsync(string firstName)
    {
        var result = collection.Where(t => t.FirstName == firstName).ToList();
        var likeString = firstName.Substring(0, 2);
        var result2 = collection.Where(t => t.FirstName.Contains(likeString)).ToList();
        return Ok(new
        {
            Result1 = result,
            Result2 = result2
        });
    }
    [HttpGet("/AgeAverange", Name = "AgeAverange")]
    public IActionResult GetAgeAverangeAsync()
    {
        var averangeAge = aggregation.Average(t => t.RecordShell!.Age);

        return Ok(averangeAge);
    }

    [HttpGet("/Aggregator", Name = "Aggregator")]
    public IActionResult GetAggregateAsync()
    {
        var averangeAge = aggregation.Average(t => t.RecordShell!.Age);
        var aggregateLastName = aggregation
                .Apply(x => string.Format("{0}", x.RecordShell!.LastName), "LastName")
                .ToArray();

        List<AggregationCustomer> agg = new List<AggregationCustomer>();
        foreach (var item in aggregateLastName)
        {
            agg.Add(new() { LastName = item["LastName"], Cout = item.Aggregations.Count });
        }

        return Ok(agg);
    }


    [HttpGet("/Age", Name = "Age")]
    public IActionResult GetAgeAsync()
    {
        var aggregateLastName = aggregation
                .Apply(x => x.RecordShell!.Age, "Age")
                .ToArray();

        List<AggregationAgeCustomer> agg = new List<AggregationAgeCustomer>();
        foreach (var item in aggregateLastName)
        {
            agg.Add(new() { Age = item["Age"], Cout = item.Aggregations.Count });
        }

        return Ok(agg);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Customer customer)
    {
        await collection.InsertAsync(customer);
        return Created($"search?firstName={customer.FirstName}", customer);
    }
}

