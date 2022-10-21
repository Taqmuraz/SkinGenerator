using System.Linq;
using UnityEngine;

public sealed class SkinClothConnectionNodeTemplate : SkinNodeTemplate
{
    [SerializeField] float thickness = 0.1f;
    [SerializeField] SkinNodeTemplate startA;
    [SerializeField] SkinNodeTemplate startB;
    [SerializeField] SkinNodeTemplate endA;
    [SerializeField] SkinNodeTemplate endB;

    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinClothConnectionNodeGenerator(MaterialIndex, thickness, startA, startB, endA, endB);
    }
}
