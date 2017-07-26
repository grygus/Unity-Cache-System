using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public static partial class Cache<T> where T : class
    {
        public class CacheRegister
        {
//            internal static event PoolStateChangedEventHandler<T> Added = (sender, arg) => { };
//            internal static event PoolStateChangedEventHandler<T> Removed = (sender, arg) => { };

            private static Dictionary<string, IPool<T>> _namedCache;

            public CacheRegister()
            {
                _namedCache = new Dictionary<string, IPool<T>>();
            }
            public IPool<T> this[string poolName]
            {
                get
                {
                    if (!_namedCache.ContainsKey(poolName))
                    {
                        _namedCache[poolName] = new CacheInternal();
                        _namedCache[poolName].Added += (sender, arg) => AddedAny(sender, arg);
                        _namedCache[poolName].Removed += (sender, arg) => RemovedAny(sender, arg);
                    }
                    return _namedCache[poolName];
                }
            }
        }
    }
}
