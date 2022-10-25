using System.Linq;
using UnityEngine;

public sealed class SkinClothConnectionNodeTemplate : SkinNodeTemplate
{
    [SerializeField] float thicknessA = 0.1f;
    [SerializeField] float thicknessB = 0.1f;
    [SerializeField] SkinNodeTemplate startA;
    [SerializeField] SkinNodeTemplate startB;
    [SerializeField] SkinNodeTemplate endA;
    [SerializeField] SkinNodeTemplate endB;

    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinClothConnectionNodeGenerator(MaterialIndex, thicknessA, thicknessB, startA, startB, endA, endB);
    }
}
