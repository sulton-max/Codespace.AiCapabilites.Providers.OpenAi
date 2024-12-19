namespace AiCapabilities.Providers.OpenAi.Capabilities.Embeddings.FeatureEncoding.Extensions;

public static class EmbeddingExtensions
{
    public static float CalculateCosineSimilarity(float[] v1, float[] v2)
    {
        var dotProduct = v1.Zip(v2, (a, b) => a * b).Sum();
        var magnitude1 = (float)Math.Sqrt(v1.Sum(x => x * x));
        var magnitude2 = (float)Math.Sqrt(v2.Sum(x => x * x));
        
        return dotProduct / (magnitude1 * magnitude2);
    }
}