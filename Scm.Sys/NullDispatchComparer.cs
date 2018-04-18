using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimpleLiveData.App.DataModel
{
    public abstract class NullDispatchComparer<T> : IComparer<T>
        where T: class
    {
        public virtual bool NullFirst { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract int CompareNonNull(T x, T y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NullSort(int x) => NullFirst ? x : -x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(T x, T y)
        {
            if ( x is null) return y is null ? 0 : NullSort(-1);
            return y is null ? NullSort(1) : CompareNonNull(x, y);
        }
    }
}