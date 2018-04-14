using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleLiveData.App.DataModel
{
    [DataContract]
    public class MyEntity
    {
        public MyEntity() { }
        public MyEntity(Guid id, string str)
        {
            Id = id;
            Str = str;
        }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Str { get; set; }
        //public ICollection<B> Bs { get; set; } = new List<B>();
    }
}