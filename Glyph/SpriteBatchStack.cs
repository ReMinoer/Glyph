using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class SpriteBatchStack
    {
        private readonly Stack<SpriteBatchContext> _contextStack;
        private readonly SpriteBatch _current;

        public SpriteBatch Current => _contextStack.Count > 0 && Peek != null ? _current : null;
        public SpriteBatchContext Peek => _contextStack.Count > 0 ? _contextStack.Peek() : null;

        public SpriteBatchStack(SpriteBatch spriteBatch)
        {
            _current = spriteBatch;
            _contextStack = new Stack<SpriteBatchContext>();
        }

        public void Push(SpriteBatchContext context)
        {
            StopPeek();
            _contextStack.Push(context);
            ApplyPeek();
        }

        public void Replace(SpriteBatchContext context)
        {
            StopPeek();
            _contextStack.Pop();
            _contextStack.Push(context);
            ApplyPeek();
        }

        public void Pop()
        {
            StopPeek();
            _contextStack.Pop();
            ApplyPeek();
        }

        private void StopPeek()
        {
            if (Peek == null)
                return;

            _current.End();
        }

        private void ApplyPeek()
        {
            if (Peek == null)
                return;

            _current.Begin(Peek.SpriteSortMode, Peek.BlendState, Peek.SamplerState, Peek.DepthStencilState, Peek.RasterizerState, Peek.Effect, Peek.TransformMatrix);
        }
    }
}