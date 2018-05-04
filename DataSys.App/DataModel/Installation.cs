using System;
using System.Collections.Generic;
using Scm.Sys;

namespace DataSys.App.DataModel
{
    public class Installation : AbstractNamedEntity
    {
        protected Installation()
        {
        }

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

        public Period InstallationPeriod
        {
            get => new Period(InstalledAt, DecommisionedAt);
            set
            {
                InstalledAt = value.From;
                DecommisionedAt = value.To;
            }
        }

        public DateTime? InstalledAt { get; set; }
        public DateTime? DecommisionedAt { get; set; }
    }
}