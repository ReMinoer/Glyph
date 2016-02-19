using Glyph.UI.Misc;

namespace Glyph.Tools.StatusDisplay
{
    public class StatusDisplayText : TextSprite
    {
        public string Label { get; set; }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (value != "")
                    base.Text = Label + " : " + value;
                else
                    base.Text = "";
            }
        }

        public override int Contour
        {
            get { return 1; }
        }

        public StatusDisplayText(string label)
        {
            Initialize();

            Label = label;
        }

        public override void LoadContent(ContentLibrary ressources)
        {
            base.LoadContent(ressources, "statusDisplayText");
        }
    }
}