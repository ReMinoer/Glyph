namespace Glyph
{
    public abstract class GlyphUpdateBase : GlyphComponent, IUpdate, IEnableable
    {
        public virtual bool Enabled { get; set; }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            UpdateThis(elapsedTime);
        }

        protected abstract void UpdateThis(ElapsedTime elapsedTime);
    }
}