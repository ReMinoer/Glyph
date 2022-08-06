using System;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Glyph.Pipeline
{
    static public class ContentProcessorUtils
    {
        static public Type GetEngineContentType(Type processorType)
        {
            var attribute = processorType.GetCustomAttribute<EngineContentAttribute>();
            if (attribute != null)
                return attribute.EngineContentType;

            switch (processorType.Name)
            {
                case nameof(TextureProcessor):
                    return typeof(Texture2D);
                case nameof(SongProcessor):
                    return typeof(Song);
                case nameof(SoundEffectProcessor):
                    return typeof(SoundEffect);
                case nameof(ModelProcessor):
                    return typeof(Model);
                case nameof(MaterialProcessor):
                    return typeof(EffectMaterial);
                case nameof(FontDescriptionProcessor):
                case nameof(FontTextureProcessor):
                case nameof(LocalizedFontProcessor):
                    return typeof(SpriteFont);
                case nameof(EffectProcessor):
                    return typeof(Effect);
                // TODO: Handle video for supporting platforms
                //case nameof(VideoProcessor): // Not supported on all platforms
                //    return typeof(Video);
                case nameof(PassThroughProcessor):
                    return typeof(object);
                default:
                    return null;
            }
        }
    }
}