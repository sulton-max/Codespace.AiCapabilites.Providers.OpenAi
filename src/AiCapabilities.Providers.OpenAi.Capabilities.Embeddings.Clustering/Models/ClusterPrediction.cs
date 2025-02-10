using Microsoft.ML.Data;

public class ClusterPrediction
{
    // The predicted cluster label (as a uint).
    [ColumnName("PredictedLabel")]
    public uint ClusterId { get; set; }

    // The distances (score) to each cluster centroid.
    [ColumnName("Score")]
    public float[] Distances { get; set; } = null!;
}