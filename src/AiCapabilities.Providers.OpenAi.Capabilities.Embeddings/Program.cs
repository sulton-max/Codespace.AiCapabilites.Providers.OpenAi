using Microsoft.Extensions.Configuration;
using OpenAI;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create an image generation client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-small");

// Generate simple embedding
var result = await embeddingClient.GenerateEmbeddingAsync("black leather jacket");
var embeddings = result.Value.ToFloats();
Console.WriteLine($"Embeddings :");
foreach(var embedding in embeddings.ToArray())
{
    Console.WriteLine(embedding);
}