using UnityEngine;

public sealed class SkinNodeData
{
    public SkinNodeData(int index, Vector3[] vertices)
    {
        Index = index;
        Vertices = vertices;
    }

    public int Index { get; }
    public Vector3[] Vertices { get; }
}
