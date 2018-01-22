using System;
using System.Collections.Generic;

namespace Glyph.Messaging
{
    public interface ISubscribableRouter : IRouter, ICollection<Delegate>
    {
        void Add<T>(Action<T> item)
            where T : IMessage;
        bool Remove<T>(Action<T> item)
            where T : IMessage;
        bool Contains<T>(Action<T> item)
            where T : IMessage;
    }
}