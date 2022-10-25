using System.Linq;
using UnityEngine;

public sealed class SkinNodeFrustumGenerator : SkinNodeTubeGenerator
{
    Vector2 startSize;
    Vector2 endSize;

    public SkinNodeFrustumGenerator(Vector2 startSize, Vector2 endSize)
    {
        this.startSize = startSize;
        this.endSize = endSize;
    }

    public override void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        var child = joint.Children.FirstOrDefault();
        if (child != null)
        {
            Quaternion rotation = joint.Matrix.rotation;
            Vector3 position = joint.Matrix.GetPosition();
            Vector3 endPosition = child.Matrix.GetPosition();

            meshDataStream.PushIndexBuffer(joint.MaterialIndex);

            meshDataStream.WriteVertexData(CreateVertices(position, rotation, startSize, Vector2.zero, joint.Index));
            meshDataStream.WriteVertexData(CreateVertices(endPosition, rotation, endSize, Vector2.up, joint.Index));
            meshDataStream.WriteIndices(LocalStartIndices);
            meshDataStream.WriteIndices(LocalIndices);
            meshDataStream.WriteIndices(LocalEndIndices);
        }
    }
}
