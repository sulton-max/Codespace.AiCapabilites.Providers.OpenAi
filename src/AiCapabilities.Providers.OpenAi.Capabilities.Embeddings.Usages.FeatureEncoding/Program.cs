using AiCapabilities.Providers.OpenAi.Capabilities.Embeddings.Usages.FeatureEncoding.Extensions;
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

var rawReviews = new List<string>
{
    "Great quality and performance",
    "Poor quality, not worth the price",
    "Decent product with some minor issues",
    "Below average, wouldn't recommend",
    "Good product, works as expected",
    "Average product, nothing special",
    "Terrible product, don't buy",
    "This is an amazing product, highly recommend",
};

var positiveAnchors = new List<string>
{
    "Excellent product, exceeded expectations, highly recommend",
    "Outstanding quality, perfect performance, best purchase ever",
    "Amazing product, incredible value, absolutely love it",
    "Fantastic features, exceptional quality, worth every penny",
    "Superb design, flawless operation, couldn't be happier"
};

var negativeAnchors = new List<string>
{
    "Terrible product, complete waste of money, avoid",
    "Poor quality, doesn't work, extremely disappointed",
    "Awful performance, many defects, total failure",
    "Completely useless, worst purchase ever, regret buying",
    "Defective product, horrible experience, stay away"
};

var positiveEmbeddings = await Task.WhenAll(positiveAnchors.Select(a =>
    embeddingClient.GenerateEmbeddingAsync(a)));
var negativeEmbeddings = await Task.WhenAll(negativeAnchors.Select(a =>
    embeddingClient.GenerateEmbeddingAsync(a)));

var reviews = await Task.WhenAll(rawReviews.Select(async review =>
{
    var embedding = await embeddingClient.GenerateEmbeddingAsync(review);
    var embedVector = embedding.Value.ToFloats().ToArray();

    var positiveSims = positiveEmbeddings.Select(e =>
        EmbeddingExtensions.CalculateCosineSimilarity(embedVector, e.Value.ToFloats().ToArray()));
    var negativeSims = negativeEmbeddings.Select(e =>
        EmbeddingExtensions.CalculateCosineSimilarity(embedVector, e.Value.ToFloats().ToArray()));

    var avgPositiveSim = positiveSims.Average();
    var avgNegativeSim = negativeSims.Average();

    var sentiment = (avgPositiveSim - avgNegativeSim);
    var score = 1 + 4 / (1 + Math.Exp(-5 * sentiment));

    return new { Content = review, Embedding = embedVector, Score = score };
}));

// Display reviews
foreach (var review in reviews)
    Console.WriteLine("Content: {0}, Score: {1:F2}", review.Content, review.Score);
