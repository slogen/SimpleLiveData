using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleLiveData.App.Presentation.Owin
{
    [DataContract]
    public class Result<T> : IResult<T>
    {
        public Result(ICollection<T> data)
        {
            Data = data;
        }

        [DataMember] public ICollection<T> Data { get; }
    }
}