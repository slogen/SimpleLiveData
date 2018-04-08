using System;
using System.Collections.Generic;

namespace SimpleLiveData.App.DataModel
{
    public class A
    {
        public Guid Id { get; set; }
        public string Str { get; set; }
        public ICollection<B> Bs { get; set; } = new List<B>();
    }
}
