using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(); // Add SignalR support
builder.Services.AddSingleton<QdrantClient>(new QdrantClient("localhost", 6334)); // Register Qdrant client as a singleton

// Add CORS services
builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); // Apply CORS before Authentication/Authorization
app.UseAuthorization();

app.MapControllers();
app.MapHub<SupportHub>("/supportHub");

app.Run();