using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class SkinNodeTemplate : MonoBehaviour, ISkinNode, ISkinJoint
{
    [SerializeField] float nodeWidth = 0.05f;
    int index;
    SkinNodeTemplate[] childTemplates;

    public void TraceIndices(List<SkinNodeTemplate> jointsList)
    {
        index = jointsList.Count;
        jointsList.Add(this);
        foreach (var child in childTemplates) child.TraceIndices(jointsList);
    }

    void Awake()
    {
        childTemplates = transform.Cast<Transform>().Select(t => t.GetComponent<SkinNodeTemplate>()).Where(t => t != null).ToArray();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nodeWidth * 0.5f);
    }

    public void AppendMeshData(ISkinNodeDataStream skinStream)
    {
        GenerateNode(skinStream);

        switch (childTemplates.Length)
        {
            case 0:
                return;
            case 1:
                var singleChild = childTemplates[0];
                singleChild.AppendMeshData(skinStream);
                break;
            default:
                foreach (var child in childTemplates)
                {
                    skinStream.PushBuffer();
                    child.AppendMeshData(skinStream);
                }
                break;
        }
    }
    void GenerateNode(ISkinNodeDataStream stream)
    {
        float halfWidth = nodeWidth * 0.5f;

        Vector3[] localVertices = new Vector3[]
        {
            new Vector3(-halfWidth, 0f, -halfWidth),
            new Vector3(-halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, -halfWidth),
        };

        stream.Write(new SkinNodeData(index, localVertices.Select(l => Matrix.MultiplyPoint3x4(l)).ToArray()));
    }

    public Matrix4x4 Matrix => transform.localToWorldMatrix;
    public int Index => index;
}
