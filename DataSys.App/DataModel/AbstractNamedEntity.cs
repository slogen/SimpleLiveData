using System;

namespace DataSys.App.DataModel
{
    public abstract class AbstractNamedEntity : AbstractEntity
    {
        protected AbstractNamedEntity()
        {
        }

        protected AbstractNamedEntity(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}