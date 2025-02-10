using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using OpenAI;
using Shared.Models;

// Create configuration builder
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var openAiApiSettings = new OpenAiApiSettings();
configuration.GetSection(nameof(OpenAiApiSettings)).Bind(openAiApiSettings);

// Create an embedding client
var openAiClient = new OpenAIClient(openAiApiSettings.ApiKey);
var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-small");

var rawReviews = new List<string>
{
    "Great quality and performance",
    "Poor quality, not worth the price",
    "Decent product with some minor issues",
    "Below average, wouldn't recommend",
    "Good product, works as expected",
    "Average product, nothing special",
    "Terrible product, don't buy",
    "This is an amazing product, highly recommend"
};

// Generate embeddings
var data = await Task.WhenAll(rawReviews.Select(async rawReview => new TextData
{
    Text = rawReview,
    Embedding = (await embeddingClient.GenerateEmbeddingAsync(rawReview)).Value.ToFloats().ToArray()
}));

// Step 2: Cluster the embeddings using ML.NET.
var mlContext = new MLContext(seed: 30);
var dataView = mlContext.Data.LoadFromEnumerable(data);

// Build the clustering pipeline.
// Here the "Features" column is created from the Embedding column.
var pipeline = mlContext.Transforms.Concatenate("Features", nameof(TextData.Embedding))
    .Append(mlContext.Transforms.NormalizeLpNorm("Features", norm: LpNormNormalizingEstimatorBase.NormFunction.L2))
    .Append(mlContext.Clustering.Trainers.KMeans(
        featureColumnName: "Features",
        numberOfClusters: 3));

// Train the model.
var model = pipeline.Fit(dataView);

// Create a prediction engine.
var predictor = mlContext.Model.CreatePredictionEngine<TextData, ClusterPrediction>(model);

// Output the cluster predictions.
Console.WriteLine("Clustering Results:");

var clusteredFeedbacks = data
    .Select(item =>
    {
        var prediction = predictor.Predict(item);

        return new
        {
            item.Text, prediction.ClusterId,
        };
    })
    .GroupBy(item => item.ClusterId);

foreach (var cluster in clusteredFeedbacks)
{
    Console.WriteLine($"Cluster {cluster.Key}:");
    foreach (var item in cluster)
        Console.WriteLine($"  {item.Text}");
    
    Console.WriteLine();
}