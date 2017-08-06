using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public interface IPool<T>
    {
        int Count { get; }
        
        event PoolStateChangedEventHandler<T> Added;
        event PoolStateChangedEventHandler<T> Removed;

        T Pop();
        void Push(T item);
        IPool<T> Generate(int count);
        IPool<T> SetFactory(Func<T> factoryFunc);
        IPool<T> SetResetAction(Action<T> resetAction);
        IPool<T> AllowExpand();
        IPool<T> DoNotExpand();
        IPool<T> AllowRecycle();
        IPool<T> DoNotRecycle();
        IPool<T> Reset();
        void Clear();
    }

    public interface IPool
    {
        string Name { get; }
    }
}