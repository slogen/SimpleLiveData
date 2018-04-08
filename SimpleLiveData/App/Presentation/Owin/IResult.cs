using System.Collections.Generic;

namespace SimpleLiveData.App.Presentation.Owin
{
    public interface IResult<T>
    {
        // Can add more stuff here  
        ICollection<T> Data { get; }
    }
}