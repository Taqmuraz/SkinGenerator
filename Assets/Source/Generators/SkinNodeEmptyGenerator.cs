public sealed class SkinNodeEmptyGenerator : ISkinNodeGenerator
{
    public static ISkinNodeGenerator Instance { get; } = new SkinNodeEmptyGenerator();

    public void Generate(IMeshDataStream meshDataStream, ISkinJoint joint) { }
}
