using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MeshDataStream : IMeshDataStream
{
    List<VertexData> buffer = new List<VertexData>();
    List<int> indices = new List<int>();
    List<ISkinJoint> joints = new List<ISkinJoint>();

    public Mesh BuildMesh()
    {
        Vector3[] vertices = buffer.Select(v => v.Position).ToArray();
        Vector3[] normals = buffer.Select(v => v.Normal).ToArray();
        Vector2[] uvs = buffer.Select(v => v.Uv).ToArray();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.name = "GeneratedMesh";

        mesh.subMeshCount = 1;
        mesh.SetTriangles(indices.ToArray(), 0);

        mesh.bindposes = this.joints.Select(j => Matrix4x4.Inverse(j.Matrix)).ToArray();
        mesh.boneWeights = Enumerable.Range(0, buffer.Count).Select(i =>
        {
            var j = buffer[i].Joints;
            var w = buffer[i].Weights;

            return new BoneWeight()
            {
                weight0 = w.x,
                weight1 = w.y,
                weight2 = w.z,
                weight3 = 0,

                boneIndex0 = j.x,
                boneIndex1 = j.y,
                boneIndex2 = j.z,
                boneIndex3 = -1,
            };
        }).ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public void PushJoints(ISkinJoint[] joints)
    {
        this.joints.AddRange(joints);
    }

    public void WriteVertexData(VertexData[] vertexData)
    {
        buffer.AddRange(vertexData);
    }

    public void WriteIndices(int[] indices)
    {
        this.indices.AddRange(indices);
    }

    public void PushIndexBuffer(out int lastIndex)
    {
        lastIndex = buffer.Count;
    }
}