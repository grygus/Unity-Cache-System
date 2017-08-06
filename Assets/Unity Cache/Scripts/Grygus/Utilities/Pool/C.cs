using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grygus.Utilities.Pool
{
    class C<T>
    {
        public T Value;
        private IPool<T> _cache;

        internal C(T value,IPool<T> cache)
        {
            this.Value = value;
            this._cache = cache;
        } 

        public void Dispose()
        {
            _cache.Push(this.Value);
        }
    }
}
