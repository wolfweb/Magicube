using Magicube.Core;
using System;
using System.Threading;

namespace Magicube.Eventbus {
    public interface IEventContext<T> where T : class {
        int                     Priority { get; }
        CancellationToken       Status   { get; }
        T                       Entity   { get; }
    }

    public class EventContext<T> : IEventContext<T> where T : class {
        public EventContext(T entity, CancellationToken cancellationToken = default) {
            TransferContext = new TransferContext();
            Status          = cancellationToken;
            Entity          = entity;
        }

        public         T                       Entity          { get; }
        public virtual CancellationToken       Status          { get; }
        public virtual int                     Priority        { get; } = 0;
        public virtual TransferContext         TransferContext { get; }
    }
}