namespace Glyph.Composition.Modelization
{
    public interface IAssetFile
    {
        string Path { get; }
        object Data { get; }
    }

    public interface IAssetFile<out TData> : IAssetFile
    {
        new TData Data { get; }
    }
}