using System;

namespace Glyph.Resolver
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