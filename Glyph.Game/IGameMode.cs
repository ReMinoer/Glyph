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
        void PreDraw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
        void DrawScene(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
        void DrawScreen(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
        void DrawPostScene(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
        void DrawPostScreen(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
        void DrawWindow(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
    }
}