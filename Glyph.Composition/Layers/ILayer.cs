namespace Glyph.Composition.Layers
{
    public interface ILayer<TLayer>
        where TLayer : class, ILayer<TLayer>
    {
        LayerRoot<TLayer> Root { get; }
        void Initialize();
        void Update(ElapsedTime elapsedTime);
    }
}