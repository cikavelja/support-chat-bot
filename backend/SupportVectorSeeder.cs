using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class SupportVectorSeeder
{
    /// <summary>
    /// Ensures the collection exists in Qdrant and seeds it with sample support topics if no data exists.
    /// </summary>
    public static async Task EnsureSupportVectorsSeededAsync(IServiceProvider services)
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
}
