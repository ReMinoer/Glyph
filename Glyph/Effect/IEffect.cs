using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IEffect
    {
        bool Actif { get; }
        bool IsEnd { get; }

        void Initialize();

        void LoadContent(ContentLibrary content);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}