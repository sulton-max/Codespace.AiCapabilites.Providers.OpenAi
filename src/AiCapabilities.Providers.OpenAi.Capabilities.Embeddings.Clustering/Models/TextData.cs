using Microsoft.ML.Data;

public class TextData
{
    public string Text { get; set; } = null!;

    // The embedding dimension here is 1536 (for text-embedding-ada-002).
    [VectorType(1536)]
    public float[] Embedding { get; set; } = null!;
}