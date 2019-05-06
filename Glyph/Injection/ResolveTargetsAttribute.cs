using System;

namespace Glyph.Injection
{
    public class ResolveTargetsAttribute : Attribute
    {
        public ResolveTargets Targets { get; }

        public ResolveTargetsAttribute(ResolveTargets targets)
        {
            Targets = targets;
        }
    }
}