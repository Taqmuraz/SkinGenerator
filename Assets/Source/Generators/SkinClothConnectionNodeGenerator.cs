using UnityEngine;

public sealed class SkinClothConnectionNodeGenerator : ISkinNodeGenerator
{
    SkinNodeTemplate startA;
    SkinNodeTemplate startB;
    SkinNodeTemplate endA;
    SkinNodeTemplate endB;
    float thicknessA;
    float thicknessB;
    int materialIndex;

    public SkinClothConnectionNodeGenerator(int materialIndex, float thicknessA, float thicknessB, SkinNodeTemplate startA, SkinNodeTemplate startB, SkinNodeTemplate endA, SkinNodeTemplate endB)
    {
        this.startA = startA;
        this.startB = startB;
        this.endA = endA;
        this.endB = endB;
        this.thicknessA = thicknessA;
        this.thicknessB = thicknessB;
        this.materialIndex = materialIndex;
    }

    public void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        Vector3 sa = startA.transform.position;
        Vector3 sb = startB.transform.position;
        Vector3 ea = endA.transform.position;
        Vector3 eb = endB.transform.position;

        Vector3 normal = Vector3.Cross(sa - sb, ea - eb).normalized;
        Vector3 normalA = normal * thicknessA;
        Vector3 normalB = normal * thicknessB;

        VertexData[] vertices = new VertexData[]
        {
            new VertexData(sa + normalA, Vector3.zero, new Vector2(0f, 0f), new Vector3Int(startA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(sb + normalB, Vector3.zero, new Vector2(0f, 1f), new Vector3Int(startB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(ea + normalA, Vector3.zero, new Vector2(1f, 0f), new Vector3Int(endA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(eb + normalB, Vector3.zero, new Vector2(1f, 1f), new Vector3Int(endB.Index, -1, -1), new Vector3(1f, 0f, 0f)),

            new VertexData(sa - normalA, Vector3.zero, new Vector2(0f, 0f), new Vector3Int(startA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(sb - normalB, Vector3.zero, new Vector2(0f, 1f), new Vector3Int(startB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(ea - normalA, Vector3.zero, new Vector2(1f, 0f), new Vector3Int(endA.Index, -1, -1), new Vector3(1f, 0f, 0f)),
            new VertexData(eb - normalB, Vector3.zero, new Vector2(1f, 1f), new Vector3Int(endB.Index, -1, -1), new Vector3(1f, 0f, 0f)),
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