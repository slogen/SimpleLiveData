using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleLiveData.App.Presentation.Owin
{
    [DataContract]
    public class Result<T> : IResult<T>
    {
        [DataMember]
        public ICollection<T> Data { get; }

        public Result(ICollection<T> data)
        {
            Data = data;
        }
    }
}
