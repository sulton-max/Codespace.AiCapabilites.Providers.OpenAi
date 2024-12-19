using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;
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
var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-large");

// Specify embedding dimensions
var embeddingOptions = new EmbeddingGenerationOptions
{
    Dimensions = 256 
};

// Generate embedding with options
var result = await embeddingClient.GenerateEmbeddingAsync("black leather jacket", embeddingOptions);
var embeddings = result.Value.ToFloats().ToArray();

// Normalize embedding value
var norm = Math.Sqrt(embeddings.Sum(x => x * x));
var normalizedEmbedding = norm == 0 ? embeddings : embeddings.Select(x => (float)(x / norm)).ToArray();

Console.WriteLine($"Embeddings :");
foreach(var embedding in normalizedEmbedding)
{
    Console.WriteLine(embedding);
}