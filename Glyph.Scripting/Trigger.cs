namespace Glyph.Scripting
{
    public class Trigger
    {
        public bool Actif { get; set; }

        public Trigger()
        {
            Actif = true;
        }

        public Trigger(bool b)
        {
            Actif = b;
        }

        public override string ToString()
        {
            return Actif ? "Enable" : "Disable";
        }
    }
}