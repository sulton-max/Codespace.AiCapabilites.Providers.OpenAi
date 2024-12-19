using System.Reflection.Metadata;
using AiCapabilities.Providers.OpenAi.Capabilities.Embeddings.TextSearch.Extensions;
using AiCapabilities.Providers.OpenAi.Capabilities.Embeddings.TextSearch.Models;
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

var articles = new List<string>
{
    """
    Microsoft Corporation is an American multinational technology conglomerate headquartered in Redmond, Washington.[2] Founded in 1975, the company became highly influential in the rise of personal computers through software like Windows, and the company has since expanded to Internet services, cloud computing, video gaming and other fields. Microsoft is the largest software maker, one of the most valuable public U.S. companies,[a] and one of the most valuable brands globally.

    Microsoft was founded by Bill Gates and Paul Allen to develop and sell BASIC interpreters for the Altair 8800. It rose to dominate the personal computer operating system market with MS-DOS in the mid-1980s, followed by Windows
    """,
    """
    Apple Inc. is an American multinational corporation and technology company headquartered and incorporated in Cupertino, California, in Silicon Valley. It is best known for its consumer electronics, software, and services. Founded in 1976 as Apple Computer Company by Steve Jobs, Steve Wozniak and Ronald Wayne, the company was incorporated by Jobs and Wozniak as Apple Computer, Inc. the following year. It was renamed Apple Inc. in 2007 as the company had expanded its focus from computers to consumer electronics. Apple is the largest technology company by revenue, with US$391.04 billion in FY 2024.

    The company was founded to produce and market Wozniak's Apple I personal computer. Its second computer, the Apple II, became a best seller as one of the first mass-produced microcomputers. Apple introduced the Lisa in 1983 and the Macintosh in 1984, as some of the first computers to use a graphical user interface and a mouse. By 1985, internal company problems led to Jobs leaving to form NeXT, Inc., and Wozniak withdrawing to other ventures
    """
};

// Generate embeddings
var documents = await Task.WhenAll(articles.Select(async article =>
{
    var result = await embeddingClient.GenerateEmbeddingAsync(article);
    return new SimpleDocument(article, result.Value.ToFloats().ToArray());
}));

// Generate query embedding
var searchQuery = "silicon valley based technology company";
var result = await embeddingClient.GenerateEmbeddingAsync(searchQuery);
var searchEmbedding = result.Value.ToFloats().ToArray();

// Get cosine similarity and order by it
var orderedDocuments = documents
    .Select(document => new
    {
        Document = document,
        Similarity = EmbeddingExtensions.CalculateCosineSimilarity(document.Embedding, searchEmbedding)
    })
    .OrderByDescending(x => x.Similarity);

Console.WriteLine("Ordered documents by similarity to search query:");
foreach (var document in orderedDocuments)
{
    Console.WriteLine($"Similarity: {document.Similarity}");
    Console.WriteLine($"Document: {document.Document.Content.Substring(0, 50)}...");
    Console.WriteLine();
}