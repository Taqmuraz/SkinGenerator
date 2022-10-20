using UnityEngine;

public sealed class SkinNodeHeadTemplate : SkinNodeTemplate
{
    [SerializeField] Vector3 size = Vector3.one;
    [SerializeField] Vector3 rotation;
    [SerializeField] Vector3 offset;

    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinNodeHeadGenerator(offset, size, Quaternion.Euler(rotation));
    }
}