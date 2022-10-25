using UnityEngine;

public sealed class VertexData
{
    public VertexData(Vector3 position, Vector3 normal, Vector2 uv, Vector3Int joints, Vector3 weights)
    {
        Position = position;
        Normal = normal;
        Uv = uv;
        Joints = joints;
        Weights = weights;
    }

    public Vector3 Position { get; }
    public Vector3 Normal { get; }
    public Vector2 Uv { get; }
    public Vector3Int Joints { get; }
    public Vector3 Weights { get; }
}
