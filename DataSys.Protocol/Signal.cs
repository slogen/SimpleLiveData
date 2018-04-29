using System;
using System.Runtime.Serialization;

namespace DataSys.Protocol
{
    [DataContract]
    public class Signal
    {
        [DataMember] public Guid Id { get; set; }

        [DataMember] public string Name { get; set; }
    }
}