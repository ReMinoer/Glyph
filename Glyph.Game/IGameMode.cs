using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Game
{
    public interface IGameMode
    {
        void Initialize();
        void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void HandleInput(InputManager inputManager);
        void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
    }
}