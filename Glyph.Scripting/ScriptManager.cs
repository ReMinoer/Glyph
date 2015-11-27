using Diese.Lua;
using Glyph.Composition;
using NLua;

namespace Glyph.Scripting
{
    public class ScriptManager : GlyphComposite<IScriptPlayer>, IUpdate
    {
        public Lua Lua { get; private set; }

        public ScriptManager()
        {
            Lua = new Lua();
            Lua.LoadCLRPackage();
            Lua.LoadCoroutineManager();
            Lua.LoadTableTools();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (IScriptPlayer scriptPlayer in this)
                scriptPlayer.Update(elapsedTime);
        }
    }
}