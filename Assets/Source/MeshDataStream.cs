using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MeshDataStream : IMeshDataStream
{
    List<VertexData> buffer = new List<VertexData>();
    List<List<int>> indices = new List<List<int>>()
    {
        new List<int>()
    };
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

        mesh.subMeshCount = indices.Count;

        for (int i = 0; i < indices.Count; i++)
        {
            mesh.SetTriangles(indices[i].ToArray(), i);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public void PushJoints(ISkinJoint[] joints)
    {
        this.joints.AddRange(joints);
    }

    public void Write(VertexData[] vertexData)
    {
        buffer.AddRange(vertexData);
    }

    public void WriteIndices(int[] indices)
    {
        this.indices[this.indices.Count - 1].AddRange(indices);
    }

    public void PushIndexBuffer()
    {
        indices.Add(new List<int>());
    }
}