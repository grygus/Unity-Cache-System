using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public static partial class Cache<T> where T : class
    {
        private class CacheInternal : IPool,IPool<T>
        {
            public string Name { get; private set; }

            private HashSet<T> _poolSet;
            private Queue<T> _poolOrder;
            private HashSet<T> _deploySet;
            private List<T> _deployOrder;

            public int Count { get { return _poolSet.Count; } }

            public event PoolStateChangedEventHandler<T> Added;
            public event PoolStateChangedEventHandler<T> Removed;

            private Func<T> _factoryMethod;
            private Action<T> _resetMethod;

            private bool _canExpand;
            private bool _canRecycle;

            public CacheInternal(string name)
            {
                Name = name;
                TryGetDefaultConstructor();
                _poolSet = new HashSet<T>();
                _poolOrder = new Queue<T>();
                _deploySet = new HashSet<T>();
                _deployOrder = new List<T>();
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
                if (_poolOrder.Count == 0)
                {
                    if (_canRecycle)
                        TryRecycle();
                    if (_canExpand)
                        Generate(1);
                }

                if (_poolOrder.Count > 0)
                {
                    var itemToPop = _poolOrder.Dequeue();
                    var isSuccess = _poolSet.Remove(itemToPop);
                    if (!isSuccess)
                    {
                        throw new NotImplementedException();
                    }
                    Removed(this, itemToPop);

                    _deploySet.Add(itemToPop);
                    _deployOrder.Add(itemToPop);
                    return itemToPop;
                }
                throw new NoObjectsInPoolException(typeof(T),Name);
            }

            public void Push(T instance)
            {
                var isSuccess = _poolSet.Add(instance);
                if (isSuccess)
                {
                    _resetMethod(instance);
                    _poolOrder.Enqueue(instance);

                    if (_deploySet.Contains(instance))
                    {
                        _deploySet.Remove(instance);
                        _deployOrder.Remove(instance);
                    }

                    Added(this, instance);
                }
                else
                {
                    throw new ObjectAlreadyInPoolException();
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

            public IPool<T> AllowExpand()
            {
                _canExpand = true;
                return this;
            }

            public IPool<T> DoNotExpand()
            {
                _canExpand = false;
                return this;
            }

            public IPool<T> AllowRecycle()
            {
                _canRecycle = true;
                return this;
            }

            public IPool<T> DoNotRecycle()
            {
                _canRecycle  = false;
                return this;
            }

            public IPool<T> Reset()
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                _poolSet.Clear();
                _poolOrder.Clear();
            }

            private void TryRecycle()
            {
                if (_deploySet.Count > 0)
                {
                    Push(_deployOrder[0]);
                }
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
