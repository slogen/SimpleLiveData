using System;

namespace Scm.DataAccess
{
    public struct Change
    {
        private struct ChangeStruct<TEntity> : IChange<TEntity>
        {
            public ChangeStruct(EntityChange change, TEntity entity)
            {
                Change = change;
                Entity = entity;
            }

            public EntityChange Change { get; }
            object IChange.Entity => Entity;

            public TEntity Entity { get; }
        }

        public static IChange<TEntity> Create<TEntity>(EntityChange change, TEntity entity)
            => new ChangeStruct<TEntity>(change, entity);

        public static IChange Create(EntityChange change, Type entityType, object entity)
            => (IChange) Activator.CreateInstance(typeof(ChangeStruct<>).MakeGenericType(entityType), change, entity);
    }
}