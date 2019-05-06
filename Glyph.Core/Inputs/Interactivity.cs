using System.Collections;
using System.Collections.Generic;
using Fingear;
using Fingear.Interactives;
using Glyph.Composition;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.Core.Inputs
{
    public abstract class InteractiveComponentBase : GlyphComponent, IEnableable
    {
        protected abstract IInteractive InteractiveComponent { get; }

        public bool Enabled
        {
            get => InteractiveComponent.Enabled;
            set => InteractiveComponent.Enabled = value;
        }

        public override void Dispose()
        {
            InteractiveComponent.Parent = null;
            base.Dispose();
        }
    }

    public class InteractiveRoot : InteractiveComponentBase
    {
        private readonly InteractiveComposite _interactiveComposite = new InteractiveComposite();
        protected override sealed IInteractive InteractiveComponent => _interactiveComposite;

        public IInteractive Interactive => InteractiveComponent;

        public InteractiveRoot([Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            if (parent?.Name != null)
                _interactiveComposite.Name = parent.Name + " interactivity";
        }

        public void Register(Controls controls)
        {
            if (_interactiveComposite.Contains(controls.Interactive))
                return;

            controls.InteractiveRoot = this;
            _interactiveComposite.Add(controls.Interactive);
        }

        public void Unregister(Controls controls)
        {
            if (_interactiveComposite.Remove(controls.Interactive))
                controls.InteractiveRoot = null;
        }
    }

    public class Controls : InteractiveComponentBase, ICollection<IControl>
    {
        private InteractiveRoot _interactiveRoot;

        public Interactive Interactive { get; } = new Interactive();
        protected override sealed IInteractive InteractiveComponent => Interactive;

        [Resolvable, ResolveTargets(ResolveTargets.Fraternal | ResolveTargets.BrowseAllAncestors)]
        public InteractiveRoot InteractiveRoot
        {
            get => _interactiveRoot;
            set
            {
                if (_interactiveRoot == value)
                    return;
                
                InteractiveRoot previousRoot = _interactiveRoot;
                _interactiveRoot = null;
                previousRoot?.Unregister(this);
                _interactiveRoot = value;
                _interactiveRoot?.Register(this);
            }
        }

        public Controls([Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            if (parent?.Name != null)
                Interactive.Name = parent.Name + " controls";
        }

        public void Add(IControl item) => Interactive.Add(item);
        public bool Remove(IControl item) => Interactive.Remove(item);
        public void Clear() => Interactive.Clear();
        public bool Contains(IControl item) => Interactive.Contains(item);
        public IEnumerator<IControl> GetEnumerator() => Interactive.GetEnumerator();

        int ICollection<IControl>.Count => ((ICollection<IControl>)Interactive).Count;
        bool ICollection<IControl>.IsReadOnly => ((ICollection<IControl>)Interactive).IsReadOnly;
        void ICollection<IControl>.CopyTo(IControl[] array, int arrayIndex) => ((ICollection<IControl>)Interactive).CopyTo(array, arrayIndex);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}