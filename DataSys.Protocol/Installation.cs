using System;
using System.Runtime.Serialization;

namespace DataSys.Protocol
{
    [DataContract]
    public class Installation
    {
        public Installation(Guid id, string name, DateTime? from, DateTime? to)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime? From { get; set; }
        [DataMember]
        public DateTime? To { get; set; }
    }
}