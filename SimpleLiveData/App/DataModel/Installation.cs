using System;
using System.Collections.Generic;
using Scm.Sys;

namespace SimpleLiveData.App.DataModel
{
    public class Installation : AbstractNamedEntity
    {
        public Installation(Guid id, string name, Period installationPeriod = null) : base(id, name)
        {
            InstallationPeriod = installationPeriod ?? new Period();
        }

        public Installation(Installation other) : this(
            other.Id,
            other.Name,
            other.InstallationPeriod)
        {
        }

        public virtual ICollection<Data> Data { get; set; } =
            new SortedSet<Data>(DataModel.Data.Comparer.ByTimeThenInstallationThenSignal);

        public Period InstallationPeriod { get; set; }
    }
}