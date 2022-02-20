using Microsoft.AspNetCore.Mvc;
using NRediSearch;
using NRediSearch.Aggregation;
using NRediSearch.Aggregation.Reducers;
using NSearchAPI.Models;

namespace NSearchAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly Client redisClient;

    public SearchController(Client redisClient)
    {
        this.redisClient = redisClient;
    }

    [HttpGet]
    public IActionResult Get(string firstName)
    {
        SearchResult res = redisClient.Search(new Query(firstName) { WithScores = true });
        List<Customer> result = new List<Customer>();
        for (int i = 0; i < res.Documents.Count; i++)
        {
            result.Add(Customer.FromDocument(res.Documents[i].GetProperties()));
        }
        return Ok(result);
    }

    [HttpGet("/Aggregator", Name = "Aggregator")]
    public IActionResult GetAggregateAsync()
    {
        AggregationBuilder aggregationBuilder = new AggregationBuilder()
            .GroupBy("@Age", Reducers.Count().As("Count"))
            .SortBy(SortedField.Descending("@Count"));
        
        AggregationResult res = redisClient.Aggregate(aggregationBuilder);
        var countResult = res.GetResults().Count;
        List<AggregationAgeCustomer> result = new List<AggregationAgeCustomer>(countResult);
        for (int i = 0; i < countResult; i++)
        {
            var row = res.GetRow(i);
            result.Add(new AggregationAgeCustomer
            {
                Age = (int)row!.Value.GetInt64("Age"),
                Count = (int)row!.Value.GetInt64("Count")
            });
        }
       
        return Ok(result);
    }

}

