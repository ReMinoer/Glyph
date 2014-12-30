﻿using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class MouseStatusDisplayChannel : StatusDisplayChannel
    {
        public MouseStatusDisplayChannel()
        {
            OriginRight = true;

            Text["curseur"] = new StatusDisplayText("Curseur");
            Text["selection"] = new StatusDisplayText("Selection");
        }

        public override void UpdateValues(GameTime gameTime, Game game)
        {
            Text["curseur"].Text = "(X = " + EditorCursor.PositionSpace.X + ", Y = " + EditorCursor.PositionSpace.Y
                                   + " )";

            if (EditorCursor.SelectionSpace.Height != 0 && EditorCursor.SelectionSpace.Width != 0)
                Text["selection"].Text = "(X = " + EditorCursor.SelectionSpace.X + ", Y = "
                                         + EditorCursor.SelectionSpace.Y + ", W = " + EditorCursor.SelectionSpace.Width
                                         + ", H = " + EditorCursor.SelectionSpace.Height + " )";
            else
                Text["selection"].Text = "";
        }
    }
}