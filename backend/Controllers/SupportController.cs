using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;

[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly IHubContext<SupportHub> _hubContext;
    private readonly QdrantClient _qdrantClient;

    public SupportController(IHubContext<SupportHub> hubContext, QdrantClient qdrantClient)
    {
        _hubContext = hubContext;
        _qdrantClient = qdrantClient; // Injected via dependency injection
    }

    [HttpPost("query")]
    public async Task<IActionResult> QuerySupport([FromBody] string userPrompt)
    {
        try
        {
            var normalizedQuery = userPrompt.ToLower().Trim();

            var generator = new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm");
            var userEmbedding = await generator.GenerateEmbeddingVectorAsync(normalizedQuery);

            var searchResults = await _qdrantClient.SearchAsync( 
                collectionName: "support_topics",
                vector: userEmbedding.ToArray(),
                limit: 10
            );

            var filteredResults = searchResults
                .Where(r => r.Score > 0.2f)
                .OrderByDescending(r => r.Score)
                .ToList();

            foreach (var result in filteredResults)
            {
                var description = ExtractPayloadString(result.Payload, "Description");
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", description);
            }

            if (!filteredResults.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "No matches found. Please contact support@xyzcompany.com.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"Error: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    private static string ExtractPayloadString(MapField<string, Value> payload, string key)
    {
        return payload.TryGetValue(key, out var value) && value.KindCase == Value.KindOneofCase.StringValue
            ? value.StringValue
            : "No description available";
    }
}