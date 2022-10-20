using UnityEngine;

public sealed class SkinNodeData
{
    public SkinNodeData(int index, int parentIndex, Vector3[] vertices)
    {
        Index = index;
        this.ParentIndex = parentIndex;
        Vertices = vertices;
    }

    public int Index { get; }
    public int ParentIndex { get; }
    public Vector3[] Vertices { get; }
}
