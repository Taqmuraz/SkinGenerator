using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinGenerator : MonoBehaviour
{
    [SerializeField] SkinNodeTemplate rootNode;
    [SerializeField] GameObject targetObject;
    [SerializeField] Material[] materials;
    [SerializeField] bool generateOnStart = true;
    [SerializeField] bool writeOnStart = true;

    Mesh lastGeneratedMesh;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateSkin();
        }
        if (writeOnStart)
        {
            WriteSkin();
        }
    }

    [ContextMenu("Generate skin")]
    void GenerateSkin()
    {
        List<SkinNodeTemplate> nodes = new List<SkinNodeTemplate>();
        rootNode.TraceIndices(nodes, null);
        var meshStream = new MeshDataStream();
        meshStream.PushJoints(nodes.ToArray());
        foreach (var node in nodes)
        {
            node.AppendMeshData(meshStream);
        }
        lastGeneratedMesh = meshStream.BuildMesh();

        var renderer = targetObject.GetComponent<SkinnedMeshRenderer>();
        if(renderer == null) renderer = targetObject.AddComponent<SkinnedMeshRenderer>();

        renderer.bones = nodes.Select(j => j.transform).ToArray();
        renderer.sharedMesh = lastGeneratedMesh;
        renderer.rootBone = rootNode.transform;
        renderer.materials = materials;
        renderer.localBounds = new Bounds(lastGeneratedMesh.bounds.center - rootNode.transform.position, lastGeneratedMesh.bounds.size);
    }
    [ContextMenu("Write skin")]
    void WriteSkin()
    {
#if UNITY_EDITOR

        var target = targetObject.transform.root.gameObject;
        var hierarchyCopy = Instantiate(target);
        hierarchyCopy.name = target.name;
        foreach (var component in hierarchyCopy.GetComponents<MonoBehaviour>()) DestroyImmediate(component);
        foreach (var childComponent in hierarchyCopy.GetComponentsInChildren<MonoBehaviour>()) DestroyImmediate(childComponent);
        var meshPath = $"Assets/Generated/{hierarchyCopy.name}_mesh.asset";
        UnityEditor.AssetDatabase.CreateAsset(lastGeneratedMesh, meshPath);

        Material[] assetMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            var materialPath = $"Assets/Generated/Material_{i}.mat";
            UnityEditor.AssetDatabase.CreateAsset(Instantiate(materials[i]), materialPath);
            assetMaterials[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        }

        var renderer = hierarchyCopy.GetComponentInChildren<SkinnedMeshRenderer>();
        renderer.sharedMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        renderer.materials = assetMaterials;

        UnityEditor.PrefabUtility.SaveAsPrefabAsset(hierarchyCopy, $"Assets/Generated/{hierarchyCopy.name}.prefab");
        DestroyImmediate(hierarchyCopy);
#endif
    }

    sealed class GizmosDrawSkinStream : IMeshDataStream
    {
        SkinGenerator generator;
        List<VertexData> vertices = new List<VertexData>();
        Dictionary<int, List<int>> indices = new Dictionary<int, List<int>>();
        int lastIndex;
        int currentSubmesh;

        public GizmosDrawSkinStream(SkinGenerator generator)
        {
            this.generator = generator;
        }

        public void PushJoints(ISkinJoint[] joints)
        {
        }

        public void WriteVertexData(VertexData[] vertexData)
        {
            vertices.AddRange(vertexData);
        }

        public void WriteIndices(int[] indices)
        {
            this.indices[currentSubmesh].AddRange(indices.Select(i => i + lastIndex));
        }

        public void PushIndexBuffer(int submeshIndex)
        {
            if (!indices.ContainsKey(submeshIndex)) indices.Add(submeshIndex, new List<int>());
            currentSubmesh = submeshIndex;
            lastIndex = vertices.Count;
        }

        public void Draw()
        {
            for (int s = 0; s < indices.Count; s++)
            {
                var submesh = indices[s];
                Gizmos.color = generator.materials[s].color;

                for (int i = 0; i < submesh.Count; i += 3)
                {
                    Vector3 p0 = vertices[submesh[i]].Position;
                    Vector3 p1 = vertices[submesh[i + 1]].Position;
                    Vector3 p2 = vertices[submesh[i + 2]].Position;

                    Gizmos.DrawLine(p0, p1);
                    Gizmos.DrawLine(p0, p2);
                    Gizmos.DrawLine(p2, p1);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        if (rootNode != null)
        {
            var nodes = new List<SkinNodeTemplate>();
            rootNode.TraceIndices(nodes, null);
            var stream = new GizmosDrawSkinStream(this);

            foreach (var node in nodes) node.AppendMeshData(stream);

            stream.Draw();
        }
        if (lastGeneratedMesh != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(lastGeneratedMesh);
        }
    }
}