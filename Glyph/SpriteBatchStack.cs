using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class SpriteBatchStack
    {
        private readonly Stack<SpriteBatchContext> _contextStack;
        public SpriteBatch Current { get; private set; }

        public SpriteBatchContext Peek
        {
            get { return _contextStack.Peek(); }
        }

        public SpriteBatchStack(SpriteBatch spriteBatch)
        {
            Current = spriteBatch;
            _contextStack = new Stack<SpriteBatchContext>();
        }

        public void Push(SpriteBatchContext context)
        {
            if (_contextStack.Any())
                Current.End();

            _contextStack.Push(context);
            ApplyPeek();
        }

        public void Replace(SpriteBatchContext context)
        {
            Pop();
            Push(context);
        }

        public void Pop()
        {
            Current.End();
            _contextStack.Pop();

            if (_contextStack.Any())
                ApplyPeek();
        }

        private void ApplyPeek()
        {
            SpriteBatchContext context = Peek;
            Current.Begin(context.SpriteSortMode, context.BlendState, context.SamplerState, context.DepthStencilState, context.RasterizerState, context.Effect, context.TransformMatrix);
        }
    }
}