using UnityEngine;

public sealed class SkinClothConnectionNodeGenerator : ISkinNodeGenerator
{
    SkinNodeTemplate startA;
    SkinNodeTemplate startB;
    SkinNodeTemplate endA;
    SkinNodeTemplate endB;
    float thickness;
    int materialIndex;

    public SkinClothConnectionNodeGenerator(int materialIndex, float thickness, SkinNodeTemplate startA, SkinNodeTemplate startB, SkinNodeTemplate endA, SkinNodeTemplate endB)
    {
        this.startA = startA;
        this.startB = startB;
        this.endA = endA;
        this.endB = endB;
        this.thickness = thickness;
        this.materialIndex = materialIndex;
    }

    public void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        Vector3 sa = startA.transform.position;
        Vector3 sb = startB.transform.position;
        Vector3 ea = endA.transform.position;
        Vector3 eb = endB.transform.position;

        Vector3 normal = Vector3.Cross(sa - sb, ea - eb).normalized * thickness;

        VertexData[] vertices = new VertexData[]
        {
            new VertexData(sa + normal, Vector3.zero, Vector2.zero, new Vector3Int(startA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(sb + normal, Vector3.zero, Vector2.zero, new Vector3Int(startB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(ea + normal, Vector3.zero, Vector2.zero, new Vector3Int(endA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(eb + normal, Vector3.zero, Vector2.zero, new Vector3Int(endB.Index, -1, -1), new Vector3(1f, 0f, 0f)),

            new VertexData(sa - normal, Vector3.zero, Vector2.zero, new Vector3Int(startA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(sb - normal, Vector3.zero, Vector2.zero, new Vector3Int(startB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(ea - normal, Vector3.zero, Vector2.zero, new Vector3Int(endA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(eb - normal, Vector3.zero, Vector2.zero, new Vector3Int(endB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
        };
        int[] indices = new int[]
        {
            0, 1, 3,
            3, 2, 0,

            7, 5, 4,
            4, 6, 7
        };
        meshDataStream.PushIndexBuffer(materialIndex);
        meshDataStream.WriteVertexData(vertices);
        meshDataStream.WriteIndices(indices);
    }
}