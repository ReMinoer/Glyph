using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Pathfinder.Tools
{
    public class PathfindingDisplay<TMove, TAction>
        where TMove : Move<TAction>
    {
        public bool Visible { get; set; }

        private readonly int _sizeCase;
        private List<TMove> _itineraire;
        private Texture2D _square;

        public PathfindingDisplay(int sizeCase)
        {
            _sizeCase = sizeCase;

            _itineraire = new List<TMove>();
            Visible = true;
        }

        public void LoadContent(ContentLibrary content)
        {
            _square = content.GetTexture("square");
        }

        public void Update(List<TMove> it)
        {
            _itineraire = it;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (TMove p in _itineraire)
                spriteBatch.Draw(_square,
                    new Rectangle(p.Destination.X * _sizeCase, p.Destination.Y * _sizeCase, _sizeCase, _sizeCase),
                    Color.Green * 0.4f);
        }
    }
}