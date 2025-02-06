using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.AI;

[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly IHubContext<SupportHub> _hubContext;
    private readonly QdrantClient _qdrantClient;

    public SupportController(IHubContext<SupportHub> hubContext, QdrantClient qdrantClient)
    {
        _hubContext = hubContext;
        _qdrantClient = qdrantClient;
    }

    [HttpPost("query")]
    public async Task<IActionResult> QuerySupport([FromBody] SupportQueryRequest request)
    {
        try
        {
            var normalizedQuery = request.UserPrompt.ToLower().Trim();

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
                await _hubContext.Clients.Client(request.ConnectionId).SendAsync("ReceiveMessage", description);
            }

            if (!filteredResults.Any())
            {
                await _hubContext.Clients.Client(request.ConnectionId).SendAsync("ReceiveMessage", "No matches found. Please contact support@xyzcompany.com.");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.Client(request.ConnectionId).SendAsync("ReceiveMessage", $"Error: {ex.Message}");
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

// Request model to include user prompt and connection ID
public class SupportQueryRequest
{
    public required string UserPrompt { get; set; }
    public required string ConnectionId { get; set; }
}
