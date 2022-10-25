public sealed class SkinEmptyNodeTemplate : SkinNodeTemplate
{
    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinNodeEmptyGenerator();
    }
}