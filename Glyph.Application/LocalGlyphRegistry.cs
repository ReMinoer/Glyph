using Diese.Injection;
using Glyph.Messaging;
using Glyph.Scripting;

namespace Glyph.Application
{
    public class LocalGlyphRegistry : DependencyRegistry
    {
        public LocalGlyphRegistry()
        {
            //Register<LocalRouter<OnEnterTrigger>>(Subsistence.Singleton);
            //Link<IRouter<OnEnterTrigger>, LocalRouter<OnEnterTrigger>>();
        }
    }
}