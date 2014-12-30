using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class EffectFondu : IEffect
    {
        public Color Color { get; private set; }
        public float Duration { get { return _opacity.Duration; } }

        private readonly TransitionFloat _opacity = new TransitionFloat();
        private readonly bool _toTransparency;
        private Texture2D _texture;

        public EffectFondu(Color color, float duration, bool toTransparency)
        {
            Color = color;
            _opacity = new TransitionFloat();
            _opacity.Init(0, 1, duration, true, toTransparency);
            _toTransparency = toTransparency;
        }

        public bool Actif { get; private set; }
        public bool IsEnd { get { return Actif && _opacity.IsEnd; } }

        public void Initialize()
        {
            Actif = true;
            _opacity.Reset(_toTransparency);
        }

        public void LoadContent(ContentLibrary content)
        {
            _texture = content.GetTexture("square");
        }

        public void Update(GameTime gameTime)
        {
            if (Actif)
                _opacity.Update(gameTime, _toTransparency);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                new Rectangle(-50, -50, (int)Resolution.VirtualSize.X + 100, (int)Resolution.VirtualSize.Y + 100),
                Color * _opacity);
        }
    }
}