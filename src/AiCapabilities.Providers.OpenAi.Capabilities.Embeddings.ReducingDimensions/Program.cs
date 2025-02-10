using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create an OpenAI client and get the embedding client for the "text-embedding-3-large" model.
// (Note: The full "text-embedding-3-large" returns a high-dimensional vector – typically 3072 dimensions.)
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-large");

// Define embedding options for reduced embeddings
var reducedEmbeddingOptions = new EmbeddingGenerationOptions { Dimensions = 512 };

// Generate 10 complex clothing descriptions.
var descriptions = new List<string>
{
    "A sleek, modern black leather jacket with asymmetrical zipper details and minimalist design.",
    "A vintage floral maxi dress with ruffled sleeves, delicate lace overlays, and a flowing skirt.",
    "An elegant cashmere sweater in deep navy, featuring intricate cable knit patterns and a relaxed fit.",
    "A tailored charcoal gray blazer with a slim silhouette, double-breasted front, and subtle pinstripes.",
    "A bohemian peasant blouse with billowy sleeves, embroidered patterns, and a soft, earthy color palette.",
    "A sporty windbreaker in vibrant neon hues, lightweight material with reflective accents and adjustable hood.",
    "A luxurious silk blouse in rich burgundy with a high collar, ruffle trim, and a slightly translucent finish.",
    "A distressed denim jacket with custom patchwork, worn-in textures, and a mix of light and dark washes.",
    "A minimalist white t-shirt made from organic cotton, featuring a subtle geometric print on the chest.",
    "A statement piece: an oversized knit cardigan in pastel shades with intricate, hand-loomed textures."
};

// For each description, generate two embeddings: one normal and one reduced.
var clothingDataTasks = descriptions.Select(async desc =>
{
    // Normal embedding (full dimensions)
    var normalResult = await embeddingClient.GenerateEmbeddingAsync(desc);
    var normalEmbedding = normalResult.Value.ToFloats().ToArray();
    normalEmbedding = Test.NormalizeEmbedding(normalEmbedding);

    // Reduced embedding (256 dimensions)
    var reducedResult = await embeddingClient.GenerateEmbeddingAsync(desc, reducedEmbeddingOptions);
    var reducedEmbedding = reducedResult.Value.ToFloats().ToArray();
    reducedEmbedding = Test.NormalizeEmbedding(reducedEmbedding);

    return new ClothingDescription
    {
        Description = desc,
        NormalEmbedding = normalEmbedding,
        ReducedEmbedding = reducedEmbedding
    };
});
var clothingData = (await Task.WhenAll(clothingDataTasks)).ToList();

// Now, take a query and generate its embeddings for both normal and reduced dimensions.
var query = "Looking for a modern, edgy leather jacket with minimalist design";

// Query normal embedding (full-dimension)
var queryNormalResult = await embeddingClient.GenerateEmbeddingAsync(query);
var queryNormalEmbedding = Test.NormalizeEmbedding(queryNormalResult.Value.ToFloats().ToArray());

// Query reduced embedding (256 dimensions)
var queryReducedResult = await embeddingClient.GenerateEmbeddingAsync(query, reducedEmbeddingOptions);
var queryReducedEmbedding = Test.NormalizeEmbedding(queryReducedResult.Value.ToFloats().ToArray());

// Perform cosine similarity search in the normal embedding collection.
var normalResults = clothingData
    .Select(cd => new
    {
        cd.Description,
        Similarity = Test.ComputeCosineSimilarity(cd.NormalEmbedding, queryNormalEmbedding)
    })
    .OrderByDescending(r => r.Similarity)
    .ToList();

// Perform cosine similarity search in the reduced embedding collection.
var reducedResults = clothingData
    .Select(cd => new
    {
        cd.Description,
        Similarity = Test.ComputeCosineSimilarity(cd.ReducedEmbedding, queryReducedEmbedding)
    })
    .OrderByDescending(r => r.Similarity)
    .ToList();

// Show the results.
Console.WriteLine("Search Results using Normal (full-dimension) Embeddings:");
foreach (var result in normalResults)
{
    Console.WriteLine($"Score: {result.Similarity:F2} | Text: {result.Description.Substring(0, 50)}");
}

Console.WriteLine("\nSearch Results using Reduced (512-dimension) Embeddings:");
foreach (var result in reducedResults)
{
    Console.WriteLine($"Score: {result.Similarity:F2} | Text: {result.Description.Substring(0, 50)}");
}

Console.ReadLine();


public class ClothingDescription
{
    public string Description { get; set; } = null!;
    public float[] NormalEmbedding { get; set; } = [];
    public float[] ReducedEmbedding { get; set; } = [];
}

public static class Test
{
    public static float[] NormalizeEmbedding(float[] embedding)
    {
        double norm = Math.Sqrt(embedding.Sum(x => x * x));
        return norm == 0 ? embedding : embedding.Select(x => (float)(x / norm)).ToArray();
    }

    // Helper: Compute cosine similarity as the dot product (assumes both vectors are normalized).
    public static float ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
            throw new ArgumentException("Vectors must be of the same length");
        float dot = 0;
        for (var i = 0; i < vectorA.Length; i++)
            dot += vectorA[i] * vectorB[i];
        return dot;
    }
}