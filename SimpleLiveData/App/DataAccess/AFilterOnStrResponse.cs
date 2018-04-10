using System;
using System.Collections.Generic;

namespace SimpleLiveData.App.DataAccess
{
    public class AFilterOnStrResponse
    {
        public AData A { get; set; }
        public IList<int> BIds { get; set; }

        public class AData
        {
            public Guid Id { get; set; }
            public string Str { get; set; }
        }
    }
}