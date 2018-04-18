using System;

namespace SimpleLiveData.App.DataModel
{
    public abstract class AbstractEntity
    {
        public Guid Id { get; private set; }

        protected AbstractEntity(Guid id)
        {
            Id = id;
        }
    }
}