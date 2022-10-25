using System.Collections.Generic;
using UnityEngine;

public interface ISkinJoint
{
    Matrix4x4 Matrix { get; }
    int Index { get; }
    int MaterialIndex { get; }
    IEnumerable<ISkinJoint> Children { get; }
}
