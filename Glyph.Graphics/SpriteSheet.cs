using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteSheet : GlyphContainer, ISpriteSheet, ILoadContent, IUpdate
    {
        private readonly SpriteLoader _spriteLoader;
        private int _currentFrame;
        private bool _loadedContent;
        public int FramesCount => Frames.Count;
        public ISpriteSheetCarver Carver { get; private set; }
        public List<Rectangle> Frames { get; private set; }
        public Rectangle? Rectangle { get; private set; }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                _currentFrame = value;
                if (_loadedContent)
                    Refresh();
            }
        }

        public string AssetPath
        {
            get { return _spriteLoader.AssetPath; }
            set { _spriteLoader.AssetPath = value; }
        }

        public Texture2D Texture
        {
            get { return _spriteLoader.Texture; }
        }

        public event Action<ISpriteSource> Loaded;

        public SpriteSheet(IContentLibrary contentLibrary)
        {
            _spriteLoader = new SpriteLoader(contentLibrary);
            _spriteLoader.Loaded += OnSpriteLoaded;

            Components.Add(_spriteLoader);

            Frames = new List<Rectangle>();
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await _spriteLoader.LoadContent(contentLibrary);
        }

        private void OnSpriteLoaded(ISpriteSource obj)
        {
            if (Carver != null)
                ApplyCarver(Carver);

            Refresh();
            _loadedContent = true;

            Loaded?.Invoke(this);
        }

        public void ApplyCarver(ISpriteSheetCarver carver)
        {
            Carver = carver;
            Frames.Clear();

            if (Texture != null)
                carver.Process(this);
        }

        public Rectangle GetFrameRectangle(int frameIndex)
        {
            return Frames[frameIndex];
        }

        public Rectangle GetDrawnRectangle()
        {
            return Rectangle ?? _spriteLoader.GetDrawnRectangle();
        }

        Vector2 ISpriteSource.GetDefaultOrigin()
        {
            return (Rectangle?.Size.ToVector2() ?? _spriteLoader.Texture.Size()) / 2;
        }

        Texture2D ISpriteSheet.GetFrameTexture(int frameIndex)
        {
            return Texture;
        }

        private void Refresh()
        {
            Rectangle = FramesCount > 0
                ? GetFrameRectangle(CurrentFrame)
                : Microsoft.Xna.Framework.Rectangle.Empty;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            _spriteLoader.Update(elapsedTime);
        }
    }
}