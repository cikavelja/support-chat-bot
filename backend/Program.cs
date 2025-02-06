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
await SupportVectorSeeder.EnsureSupportVectorsSeededAsync(app.Services);
app.Run();

