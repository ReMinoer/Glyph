namespace Glyph.Scripting
{
    public struct ScriptLine
    {
        public object[] Args;
        public string Cmd;

        public ScriptLine(string c, object[] a)
        {
            Cmd = c;
            Args = a;
        }
    }
}