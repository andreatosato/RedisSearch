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
            { "Text", string.Join("", LoremNET.Lorem.Paragraphs(1,100, 1,100, 1, 2)) }
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

    [HttpGet("Document")]
    public async Task<IActionResult> SearchDocument(string text)
    {
        var result = await redisClient.SearchAsync(new Query(text) { WithScores = true });
        List<DocumentResult> documents = new List<DocumentResult>();
        result.Documents.ForEach(t => {
            documents.Add(new DocumentResult
            {
                Text = t["Text"],
                Score = t.Score,
                ScoreExplained = t.ScoreExplained,
                Id = t.Id
            });
        });
        return Ok(new
        {
            Count = documents.Count,
            Texts = documents.OrderByDescending(t => t.Score)
        });
    }
}

public class DocumentResult
{
    public string Text { get; set; } = null!;
    public string[] ScoreExplained { get; set; } = null!;
    public double Score { get; set; }
    public string Id { get; set; } = null!;
}
