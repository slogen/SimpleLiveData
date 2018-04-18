using System;

namespace SimpleLiveData.App.DataModel
{
    public abstract class AbstractNamedEntity : AbstractEntity
    {
        public string Name { get; set; }

        protected AbstractNamedEntity(Guid id, string name): base(id)
        {
            Name = name;
        }
    }
}