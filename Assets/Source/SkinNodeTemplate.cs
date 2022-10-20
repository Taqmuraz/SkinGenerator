using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public abstract class SkinNodeTemplate : MonoBehaviour, ISkinNode, ISkinJoint
{
    int index;
    SkinNodeTemplate[] childTemplates;
    int transformChildren;
    SkinNodeTemplate parent;
    public IEnumerable<ISkinJoint> Children => childTemplates;

    public void TraceIndices(List<SkinNodeTemplate> jointsList, SkinNodeTemplate parent)
    {
        this.parent = parent;
        index = jointsList.Count;
        jointsList.Add(this);
        foreach (var child in childTemplates) child.TraceIndices(jointsList, this);
    }
    void TraceTransform(Transform transform, List<SkinNodeTemplate> templates)
    {
        var template = transform.GetComponent<SkinNodeTemplate>();
        if (template != null) templates.Add(template);
        else foreach (Transform child in transform) TraceTransform(child, templates);
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
        var traceList = new List<SkinNodeTemplate>();
        foreach (Transform child in transform) TraceTransform(child, traceList);
        childTemplates = traceList.ToArray();
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

    protected abstract ISkinNodeGenerator CreateGenerator();

    public void AppendMeshData(IMeshDataStream stream)
    {
        CreateGenerator().Generate(stream, this);
    }

    public Matrix4x4 Matrix => transform.localToWorldMatrix;
    public int Index => index;
}
