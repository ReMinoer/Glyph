using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class SpriteBatchStack
    {
        private readonly Stack<SpriteBatchContext> _contextStack;
        private readonly SpriteBatch _current;

        public SpriteBatch Current => _contextStack.Count > 0 ? _current : null;
        public SpriteBatchContext Peek => _contextStack.Peek();

        public SpriteBatchStack(SpriteBatch spriteBatch)
        {
            _current = spriteBatch;
            _contextStack = new Stack<SpriteBatchContext>();
        }

        public void Push(SpriteBatchContext context)
        {
            if (_contextStack.Count > 0)
                StopPeek();

            _contextStack.Push(context);
            ApplyPeek();
        }

        public void Replace(SpriteBatchContext context)
        {
            _contextStack.Pop();
            StopPeek();
            _contextStack.Push(context);
            ApplyPeek();
        }

        public void Pop()
        {
            _contextStack.Pop();
            StopPeek();
            ApplyPeek();
        }

        private void StopPeek()
        {
            _current.End();
        }

        private void ApplyPeek()
        {
            if (_contextStack.Count <= 0)
                return;

            _current.Begin(Peek.SpriteSortMode, Peek.BlendState, Peek.SamplerState, Peek.DepthStencilState, Peek.RasterizerState, Peek.Effect, Peek.TransformMatrix);
        }
    }
}