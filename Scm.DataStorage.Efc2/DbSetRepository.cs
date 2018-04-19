﻿using Microsoft.EntityFrameworkCore;

namespace Scm.DataStorage.Efc2
{
    public class DbSetRepository<TEntity> : AbstractDbContextRepository<TEntity>
        where TEntity : class
    {
        public DbSetRepository(DbSet<TEntity> set)
        {
            Set = set;
        }

        protected override DbSet<TEntity> Set { get; }
    }
}