using Microsoft.AspNetCore.Mvc;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.AI;

[ApiController]
[Route("api/[controller]")]
public class SupportAdminController : ControllerBase
{
    private readonly QdrantClient _qdrantClient;
    private readonly OllamaEmbeddingGenerator _embeddingGenerator;

    public SupportAdminController(QdrantClient qdrantClient, OllamaEmbeddingGenerator embeddingGenerator)
    {
        _qdrantClient = qdrantClient;
        _embeddingGenerator = embeddingGenerator;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddSupportTopic([FromBody] SupportTopicRequest request)
    {
        try
        {
            // Generate embedding vector
            var embeddingVector = await _embeddingGenerator.GenerateEmbeddingVectorAsync(request.Title + " " + request.Description);


            ulong numericPointId = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // Create a new point to store in Qdrant
            var point = new PointStruct
            {
                Id = new PointId { Num = numericPointId },
                Vectors = new Vectors { Vector = new Vector { Data = { embeddingVector.ToArray() } } }, // Corrected vector assignment
                Payload =
                {
                    { "Title", request.Title },
                    { "Description", request.Description }
                }
            };

            // Insert the point into Qdrant
            await _qdrantClient.UpsertAsync(
                collectionName: "support_topics",
                points: new[] { point }
            );

            return Ok(new { message = "Support topic added successfully to Qdrant!", id = numericPointId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error adding support topic: {ex.Message}");
        }
    }

    public class SupportTopicRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}
