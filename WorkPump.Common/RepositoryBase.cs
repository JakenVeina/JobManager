using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WorkPump.Common
{
    public interface IRepository<TEntity, TId>
        where TEntity : EntityBase<TId>
        where TId : IEquatable<TId>
    {
        int Count { get; }

        TEntity Get(TId id);

        TEntity? TryGet(TId id);

        bool Insert(TEntity entity);

        bool Insert(IEnumerable<TEntity> entities);

        bool Remove(TId id);

        bool Remove(IEnumerable<TId> ids);
    }

    public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : EntityBase<TId>
        where TId : IEquatable<TId>
    {
        public int Count
            => _entitiesByKey.Count;

        public TEntity Get(TId id)
            => _entitiesByKey[id];

        public TEntity? TryGet(TId id)
            => _entitiesByKey.TryGetValue(id, out var entity)
                ? entity
                : null;

        public virtual bool Insert(TEntity entity)
        {
            var oldCount = _entitiesByKey.Count;

            _entitiesByKey = _entitiesByKey.Add(entity.Id, entity);

            return _entitiesByKey.Count != oldCount;
        }

        public virtual bool Insert(IEnumerable<TEntity> entities)
        {
            var oldCount = _entitiesByKey.Count;

            _entitiesByKey = _entitiesByKey.AddRange(entities, entity => entity.Id);

            return _entitiesByKey.Count != oldCount;
        }

        public virtual bool Remove(TId id)
        {
            var oldCount = _entitiesByKey.Count;

            _entitiesByKey = _entitiesByKey.Remove(id);

            return _entitiesByKey.Count != oldCount;
        }

        public virtual bool Remove(IEnumerable<TId> ids)
        {
            var oldCount = _entitiesByKey.Count;

            _entitiesByKey = _entitiesByKey.RemoveRange(ids);

            return _entitiesByKey.Count != oldCount;
        }

        private ImmutableHashDictionary<TId, TEntity> _entitiesByKey
            = ImmutableHashDictionary<TId, TEntity>.Empty;
    }
}
