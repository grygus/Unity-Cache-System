using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public static partial class Cache<T> where T : class
    {
        private class CacheInternal : IPool<T>
        {
            public HashSet<T> _poolSet;
            public Queue<T> _poolOrder;

            public int Count { get { return _poolSet.Count; } }

            public event PoolStateChangedEventHandler<T> Added;
            public event PoolStateChangedEventHandler<T> Removed;

            private Func<T> _factoryMethod;
            private Action<T> _resetMethod;

            public CacheInternal()
            {
                TryGetDefaultConstructor();
                _poolSet = new HashSet<T>();
                _poolOrder = new Queue<T>();
                _resetMethod = instance => { };
            }

            public IPool<T> Generate(int count)
            {
                if (_factoryMethod == null)
                    _factoryMethod = TryGetDefaultConstructor();
                for (int i = 0; i < count; i++)
                {
                    var item = _factoryMethod();
                    _resetMethod(item);
                    Push(item);
                }
                return this;
            }

            public T Pop()
            {
                if (_poolOrder.Count > 0)
                {
                    var itemToPop = _poolOrder.Dequeue();
                    var isSuccess = _poolSet.Remove(itemToPop);
                    if (!isSuccess)
                    {
                        throw new NotImplementedException();
                    }
                    Removed(this, itemToPop);
                    return itemToPop;
                }
                throw new NotImplementedException();
            }

            public void Push(T instance)
            {
                var isSuccess = _poolSet.Add(instance);
                if (isSuccess)
                {
                    _resetMethod(instance);
                    _poolOrder.Enqueue(instance);
                    Added(this, instance);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            public IPool<T> SetFactory(Func<T> factoryFunc)
            {
                _factoryMethod = factoryFunc;
                return this;
            }

            public IPool<T> SetResetAction(Action<T> resetAction)
            {
                _resetMethod = resetAction;
                return this;
            }

            public void Clear()
            {
                _poolSet.Clear();
                _poolOrder.Clear();
            }

            private Func<T> TryGetDefaultConstructor()
            {
                var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    var lambda = Expression.Lambda<Func<T>>(Expression.New(constructor));
                    return lambda.Compile();
                }
                else
                {
                    throw new Exception("No Default Constructor found for type:"+typeof(T).Name);
                }
            }

        }
    }
}
