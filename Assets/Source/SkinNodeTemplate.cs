using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public sealed class SkinNodeTemplate : MonoBehaviour, ISkinNode, ISkinJoint
{
    [SerializeField] float nodeWidth = 0.05f;
    int index;
    SkinNodeTemplate[] childTemplates;
    int transformChildren;

    public void TraceIndices(List<SkinNodeTemplate> jointsList)
    {
        index = jointsList.Count;
        jointsList.Add(this);
        foreach (var child in childTemplates) child.TraceIndices(jointsList);
    }

    [ContextMenu("Reset template")]
    void ResetTemplate()
    {
        RebindChildren();
        foreach (var child in childTemplates) child.RebindChildren();
    }
    void Awake()
    {
        RebindChildren();
    }
    void RebindChildren()
    {
        childTemplates = transform.Cast<Transform>().Select(t => t.GetComponent<SkinNodeTemplate>()).Where(t => t != null).ToArray();
    }

    void Update()
    {
        if (childTemplates.Any(c => c == null)) RebindChildren();
        if (transformChildren != transform.childCount)
        {
            RebindChildren();
            transformChildren = transform.childCount;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nodeWidth * 0.5f);
        foreach (var child in childTemplates)
        {
            Gizmos.DrawLine(transform.position, child.transform.position);
        }
    }

    public void AppendMeshData(ISkinNodeDataStream skinStream)
    {
        switch (childTemplates.Length)
        {
            case 0:
                GenerateNode(skinStream);
                return;
            case 1:
                GenerateNode(skinStream);
                var singleChild = childTemplates[0];
                singleChild.AppendMeshData(skinStream);
                break;
            default:
                foreach (var child in childTemplates)
                {
                    skinStream.PushBuffer();
                    GenerateNode(skinStream);
                    child.AppendMeshData(skinStream);
                }
                break;
        }
    }
    void GenerateNode(ISkinNodeDataStream stream)
    {
        if (stream is SkinNodeDataStream) Debug.Log(name);

        float halfWidth = nodeWidth * 0.5f;

        Quaternion localRotation;
        Vector3 delta = transform.localPosition;

        foreach (var child in childTemplates)
        {
            delta += child.transform.localPosition;
        }
        delta.Normalize();

        localRotation = Quaternion.FromToRotation(Vector3.up, delta);

        Vector3[] localVertices = new Vector3[]
        {
            new Vector3(-halfWidth, 0f, -halfWidth),
            new Vector3(-halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, -halfWidth),
        };

        stream.Write(new SkinNodeData(index, localVertices.Select(l => Matrix.MultiplyPoint3x4(localRotation * l)).ToArray()));
    }

    public Matrix4x4 Matrix => transform.localToWorldMatrix;
    public int Index => index;
}
