using System;

namespace SimpleLiveData.App.DataModel
{
    public abstract class AbstractNamedEntity : AbstractEntity
    {
        protected AbstractNamedEntity(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}