using System.Reactive.Subjects;
using SimpleLiveData.App.DataModel;

namespace SimpleLiveData.App.DataStorage
{

    public class Storage
    {
        public ISubject<A> A { get; } = new Subject<A>();
    }
}
