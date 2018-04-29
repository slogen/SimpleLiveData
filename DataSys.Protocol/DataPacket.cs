using System;
using System.Runtime.Serialization;

namespace DataSys.Protocol
{
    [DataContract]
    public class DataPacket
    {
        [DataMember] public Guid Type { get; set; }

        [DataMember] public byte[] Content { get; set; }
    }
}