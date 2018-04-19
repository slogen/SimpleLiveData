using System;
using System.Collections.Generic;

namespace SimpleLiveData.App.DataModel
{
    public class Signal : AbstractNamedEntity
    {
        public Signal(Guid id, string name) : base(id, name)
        {
        }

        public Signal(Signal other) : this(other.Id, other.Name)
        {
        }

        public virtual ICollection<Data> Data { get; set; } =
            new SortedSet<Data>(DataModel.Data.Comparer.ByTimeThenInstallationThenSignal);
    }
}