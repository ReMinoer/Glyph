using System.Collections.Generic;
using Glyph.Composition;

namespace Glyph.Animation
{
    public class Animator<T> : GlyphComposite<IAnimationPlayer>, IUpdate
        where T : class
    {
        public T Animatable { get; private set; }

        public Animator(T animatable)
        {
            Animatable = animatable;
        }

        public StandardAnimationPlayer<T> Add(StandardAnimation<T> standardAnimation)
        {
            var player = new StandardAnimationPlayer<T>(Animatable)
            {
                Animation = standardAnimation
            };

            Add(player);
            return player;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (Count == 0)
                return;

            var playersToRemove = new List<IAnimationPlayer>();

            foreach (IAnimationPlayer player in this)
            {
                player.Update(elapsedTime);

                if (player.Ended)
                    playersToRemove.Add(player);
            }

            foreach (IAnimationPlayer player in playersToRemove)
                Remove(player);
        }
    }
}