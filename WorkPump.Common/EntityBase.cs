using System;

namespace WorkPump.Common
{
    public abstract class EntityBase<TId>
        where TId : IEquatable<TId>
    {
        internal protected EntityBase(TId id)
        {
            Id = id;
        }

        public TId Id { get; }
    }
}
