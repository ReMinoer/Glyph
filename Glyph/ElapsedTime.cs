using Microsoft.Xna.Framework;

namespace Glyph
{
    public class ElapsedTime
    {
        static private GameTime _instance;

        static public GameTime Instance
        {
            get { return _instance; }
            set
            {
                if (_instance == null)
                    _instance = value;
            }
        }

        protected ElapsedTime()
        {
        }
    }
}