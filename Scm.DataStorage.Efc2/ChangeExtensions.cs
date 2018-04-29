using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Scm.DataAccess;

namespace Scm.DataStorage.Efc2
{
    public static class ChangeExtensions
    {
        public static EntityChange ToEntityChange(this EntityState state)
        {
            switch (state)
            {
                case EntityState.Added: return EntityChange.Add;
                case EntityState.Deleted: return EntityChange.Delete;
                case EntityState.Modified: return EntityChange.Modify;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    throw new NotSupportedException($"Cannot convert {state} to {typeof(EntityChange)}");
                default:
                    throw new NotSupportedException($"Cannot convert {state} to {typeof(EntityChange)}");
            }
        }

        public static IChange ToChange(this EntityEntry entry)
            => Change.Create(entry.State.ToEntityChange(), entry.Entity.GetType(), entry.Entity);
    }
}