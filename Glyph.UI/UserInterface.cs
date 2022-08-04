using System;
using Fingear.Interactives.Interfaces.Base;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core.Inputs;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Niddle.Attributes;

namespace Glyph.UI
{
    public class UserInterface : InteractiveChildComponentBase<UserInterface.SubscribableInterface, IGlyphInteractiveInterface>
    {
        public override sealed SubscribableInterface Interactive { get; }

        public event EventHandler<CursorEventArgs> CursorMoved;
        public event EventHandler<HandlableHoverEventArgs> CursorHovering;
        public event EventHandler<HandlableTouchEventArgs> TouchStarted;
        public event EventHandler<CursorEventArgs> Touching;
        public event EventHandler<CursorEventArgs> TouchEnded;
        public event EventHandler<HandlableDirectionEventArgs> DirectionChanged;
        public event EventHandler<HandlableEventArgs> Confirmed;
        public event EventHandler<HandlableEventArgs> Cancelled;
        public event EventHandler<HandlableEventArgs> Exited;

        public UserInterface([Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            Interactive = new SubscribableInterface(this);
            
            if (parent?.Name != null)
                Interactive.Name = parent.Name + " interface";
        }

        public override void Dispose()
        {
            CursorMoved = null;
            CursorHovering = null;
            TouchStarted = null;
            Touching = null;
            TouchEnded = null;
            DirectionChanged = null;
            Confirmed = null;
            Cancelled = null;
            Exited = null;
            base.Dispose();
        }

        public class SubscribableInterface : InteractiveInterfaceCompositeBase<IGlyphInteractiveInterface>, IGlyphInteractiveInterface
        {
            private readonly UserInterface _owner;

            public MouseCursor HoverCursor { get; private set; }
            public MouseCursor TouchCursor { get; private set; }

            public SubscribableInterface(UserInterface owner)
            {
                _owner = owner;
            }

            protected override void OnLocalCursorMoved(System.Numerics.Vector2 cursorPosition)
            {
                _owner.CursorMoved?.Invoke(this, new CursorEventArgs(cursorPosition.AsMonoGameVector()));
            }

            protected override IGlyphInteractiveInterface OnLocalCursorHovering(System.Numerics.Vector2 cursorPosition)
            {
                HoverCursor = null;

                var eventArgs = new HandlableHoverEventArgs(cursorPosition.AsMonoGameVector());
                IGlyphInteractiveInterface handler = OnHandlableEventWithHandler(_owner.CursorHovering, eventArgs);

                HoverCursor = eventArgs.Cursor;
                return handler;
            }

            protected override IGlyphInteractiveInterface OnLocalTouchStarted(System.Numerics.Vector2 cursorPosition)
            {
                var eventArgs = new HandlableTouchEventArgs(cursorPosition.AsMonoGameVector());
                IGlyphInteractiveInterface handler = OnHandlableEventWithHandler(_owner.TouchStarted, eventArgs);

                TouchCursor = eventArgs.Cursor;
                return handler;
            }

            protected override void OnLocalTouching(System.Numerics.Vector2 cursorPosition)
            {
                _owner.Touching?.Invoke(this, new CursorEventArgs(cursorPosition.AsMonoGameVector()));
            }

            protected override void OnLocalTouchEnded(System.Numerics.Vector2 cursorPosition)
            {
                _owner.TouchEnded?.Invoke(this, new CursorEventArgs(cursorPosition.AsMonoGameVector()));
            }

            protected override bool OnLocalDirectionMoved(System.Numerics.Vector2 direction)
            {
                return OnHandlableEvent(_owner.DirectionChanged, new HandlableDirectionEventArgs(direction.AsMonoGameVector()));
            }

            protected override bool OnLocalConfirmed()
            {
                return OnHandlableEvent(_owner.Confirmed, new HandlableEventArgs());
            }

            protected override bool OnLocalCancelled()
            {
                return OnHandlableEvent(_owner.Cancelled, new HandlableEventArgs());
            }

            protected override bool OnLocalExited()
            {
                return OnHandlableEvent(_owner.Exited, new HandlableEventArgs());
            }

            private bool OnHandlableEvent<TEventArgs>(EventHandler<TEventArgs> eventHandler, TEventArgs eventArgs)
                where TEventArgs : HandlableEventArgs
            {
                eventHandler?.Invoke(this, eventArgs);
                return eventArgs.IsHandled;
            }

            private IGlyphInteractiveInterface OnHandlableEventWithHandler<TEventArgs>(EventHandler<TEventArgs> eventHandler, TEventArgs eventArgs)
                where TEventArgs : HandlableEventArgs
            {
                eventHandler?.Invoke(this, eventArgs);
                return eventArgs.IsHandled ? this : null;
            }
        }
    }

    public class HandlableEventArgs : EventArgs
    {
        public bool IsHandled { get; private set; }

        public void Handle()
        {
            if (IsHandled)
                throw new InvalidOperationException();

            IsHandled = true;
        }
    }

    public class HandledEventArgs : EventArgs
    {
        public IGlyphInteractiveInterface Handler { get; }

        public HandledEventArgs(IGlyphInteractiveInterface handler)
        {
            Handler = handler;
        }
    }

    public class CursorEventArgs : EventArgs
    {
        public Vector2 CursorPosition { get; }

        public CursorEventArgs(Vector2 cursorPosition)
        {
            CursorPosition = cursorPosition;
        }
    }

    public class HandlableHoverEventArgs : HandlableEventArgs
    {
        public Vector2 CursorPosition { get; }
        public MouseCursor Cursor { get; private set; }

        public HandlableHoverEventArgs(Vector2 cursorPosition)
        {
            CursorPosition = cursorPosition;
        }

        public void Handle(MouseCursor cursor)
        {
            Cursor = cursor;
            Handle();
        }
    }

    public class HandlableTouchEventArgs : HandlableEventArgs
    {
        public Vector2 CursorPosition { get; }
        public MouseCursor Cursor { get; private set; }

        public HandlableTouchEventArgs(Vector2 cursorPosition)
        {
            CursorPosition = cursorPosition;
        }

        public void Handle(MouseCursor cursor)
        {
            Cursor = cursor;
            Handle();
        }
    }

    public class HandlableDirectionEventArgs : HandlableEventArgs
    {
        public Vector2 Direction { get; }

        public HandlableDirectionEventArgs(Vector2 direction)
        {
            Direction = direction;
        }
    }

    public class HandledHoverEventArgs : HandledEventArgs
    {
        public Vector2 CursorPosition { get; }

        public HandledHoverEventArgs(IGlyphInteractiveInterface handler, Vector2 cursorPosition)
            : base(handler)
        {
            CursorPosition = cursorPosition;
        }
    }

    public class HandledTouchEventArgs : HandledEventArgs
    {
        public Vector2 CursorPosition { get; }

        public HandledTouchEventArgs(IGlyphInteractiveInterface handler, Vector2 cursorPosition)
            : base(handler)
        {
            CursorPosition = cursorPosition;
        }
    }

    public class HandledDirectionEventArgs : HandledEventArgs
    {
        public Vector2 Direction { get; }

        public HandledDirectionEventArgs(IGlyphInteractiveInterface handler, Vector2 direction)
            : base(handler)
        {
            Direction = direction;
        }
    }
}