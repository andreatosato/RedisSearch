using Microsoft.AspNetCore.Mvc;
using NRediSearch;
using StackExchange.Redis;

namespace NSearchAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class LoremController : ControllerBase
{
    private readonly Client redisClient;

    public LoremController(SearchClients clients)
    {
        this.redisClient = clients.LoremClient;
    }

    [HttpPost]
    public async Task<IActionResult> Generate()
    {
        var d = new Document(Guid.NewGuid().ToString(), new Dictionary<string, RedisValue>()
        {
            { "Text", string.Join("", LoremNET.Lorem.Paragraphs(100, 10_000, 100, 100)) }
        });

        await redisClient.AddDocumentAsync(d);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string text)
    {
        var result = await redisClient.SearchAsync(new Query(text) { WithScores = true });
        
        return Ok(result);
    }
}
