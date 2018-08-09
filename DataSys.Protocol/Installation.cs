using System;
using System.Runtime.Serialization;

namespace DataSys.Protocol
{
    [DataContract]
    public class Installation
    {
        public Installation() { }
        public Installation(Guid id, string name, DateTimeOffset? from, DateTimeOffset? to)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
        }

        [DataMember] public Guid Id { get; set; }

        [DataMember] public string Name { get; set; }

        [DataMember] public DateTimeOffset? From { get; set; }

        [DataMember] public DateTimeOffset? To { get; set; }
    }
}