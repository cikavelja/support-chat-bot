using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));
// Register QdrantClient
builder.Services.AddSingleton<QdrantClient>(_ =>
    new QdrantClient(new Uri("http://localhost:6334"))); // Ensure Qdrant is running

builder.Services.AddSingleton(sp =>
new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

app.MapControllers();
app.MapHub<SupportHub>("/supportHub");

// **Run Seeding Before API Starts Handling Requests**
await EnsureSupportVectorsSeededAsync(app.Services);

app.Run();

/// <summary>
/// Ensures the collection exists in Qdrant and seeds it with sample support topics if no data exists.
/// </summary>
async Task EnsureSupportVectorsSeededAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var qdrantClient = scope.ServiceProvider.GetRequiredService<QdrantClient>();
    const string collectionName = "support_topics";

    // Check if the collection exists
    var collectionsResponse = await qdrantClient.ListCollectionsAsync();
    if (!collectionsResponse.Any(c => c == collectionName))
    {
        Console.WriteLine($"Creating collection: {collectionName}...");
        await qdrantClient.CreateCollectionAsync(
            collectionName: collectionName,
            vectorsConfig: new VectorParams
            {
                Size = 384,
                Distance = Distance.Cosine
            }
        );
        Console.WriteLine("Collection created successfully.");
    }
    else
    {
        Console.WriteLine("Collection already exists.");
    }

    // Seed support vectors if collection exists
    Console.WriteLine("Seeding sample support data...");
    IEmbeddingGenerator<string, Embedding<float>> generator =
    new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "all-minilm");

    var sampleData = SupportFactory.GetSupportVectorList();
    List<PointStruct> points = new();
    ulong pointId = 1;

    foreach (var topic in sampleData)
    {
        topic.Vector = await generator.GenerateEmbeddingVectorAsync(topic.Description);
        Console.WriteLine($"Generated embedding for: {topic.Description[..50]}...");

        var point = new PointStruct
        {
            Id = new PointId { Num = pointId++ },
            Vectors = new Vectors { Vector = new Vector { Data = { topic.Vector.ToArray() } } },
            Payload = { { "Description", topic.Description } }
        };

        points.Add(point);
    }

    await qdrantClient.UpsertAsync(collectionName, points);
    Console.WriteLine("Sample data seeded successfully.");
}
