﻿using Glyph.Composition;

namespace Glyph.Tools
{
    public interface IIntegratedEditor : IEnableable
    {
        object EditedObject { get; }
    }

    public interface IIntegratedEditor<out T> : IIntegratedEditor
    {
        new T EditedObject { get; }
    }
}