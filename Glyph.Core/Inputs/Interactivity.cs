using System.Collections;
using System.Collections.Generic;
using Fingear;
using Fingear.Interactives;
using Glyph.Composition;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.Core.Inputs
{
    public interface IInteractiveComponent : IEnableable
    {
        IInteractive Interactive { get; }
    }

    public interface IInteractiveComponent<out TInteractive> : IInteractiveComponent
        where TInteractive : IInteractive
    {
        new TInteractive Interactive { get; }
    }

    public abstract class InteractiveComponentBase<TInteractive> : GlyphComponent, IInteractiveComponent<TInteractive>
        where TInteractive : class, IInteractive
    {
        public abstract TInteractive Interactive { get; }
        IInteractive IInteractiveComponent.Interactive => Interactive;

        public bool Enabled
        {
            get => Interactive.Enabled;
            set => Interactive.Enabled = value;
        }

        public override void Dispose()
        {
            Interactive.Parent = null;
            base.Dispose();
        }
    }

    public abstract class InteractiveChildComponentBase<TInteractive, TInteractiveBase> : InteractiveComponentBase<TInteractive>
        where TInteractive : class, TInteractiveBase
        where TInteractiveBase : class, IInteractive
    {
        private IInteractiveComponent<IInteractiveComposite<TInteractiveBase>> _interactiveParent;
        [Resolvable, ResolveTargets(ResolveTargets.Fraternal | ResolveTargets.BrowseAllAncestors)]
        public IInteractiveComponent<IInteractiveComposite<TInteractiveBase>> InteractiveParent
        {
            get => _interactiveParent;
            set
            {
                if (_interactiveParent == value)
                    return;
                
                _interactiveParent?.Interactive.Remove(Interactive);
                _interactiveParent = value;
                if (_interactiveParent != null && !_interactiveParent.Interactive.Contains(Interactive))
                    _interactiveParent.Interactive.Add(Interactive);
            }
        }
    }

    public class InteractiveRoot : InteractiveComponentBase<InteractiveComposite>
    {
        public override sealed InteractiveComposite Interactive { get; } = new InteractiveComposite();

        public InteractiveRoot([Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            if (parent?.Name != null)
                Interactive.Name = parent.Name + " interactivity";
        }
    }

    public class Controls : InteractiveChildComponentBase<Interactive, IInteractive>, ICollection<IControl>
    {
        public override sealed Interactive Interactive { get; } = new Interactive();

        public Controls([Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            if (parent?.Name != null)
                Interactive.Name = parent.Name + " controls";
        }

        public void Add(IControl item) => Interactive.Controls.Add(item);
        public bool Remove(IControl item) => Interactive.Controls.Remove(item);
        public void Clear() => Interactive.Controls.Clear();
        public bool Contains(IControl item) => Interactive.Controls.Contains(item);
        public IEnumerator<IControl> GetEnumerator() => Interactive.Controls.GetEnumerator();

        private ICollection<IControl> InteractiveControlsCollection => Interactive.Controls;
        int ICollection<IControl>.Count => InteractiveControlsCollection.Count;
        bool ICollection<IControl>.IsReadOnly => InteractiveControlsCollection.IsReadOnly;
        void ICollection<IControl>.CopyTo(IControl[] array, int arrayIndex) => InteractiveControlsCollection.CopyTo(array, arrayIndex);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}