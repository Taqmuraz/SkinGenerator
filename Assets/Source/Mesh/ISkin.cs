using System.Collections.Generic;

public interface ISkin
{
    IEnumerable<ISkinMesh> Meshes { get; }
}
