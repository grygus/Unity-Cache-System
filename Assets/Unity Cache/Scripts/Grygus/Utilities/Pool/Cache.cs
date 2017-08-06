using System;
using System.Collections.Generic;
using System.Linq;

namespace Grygus.Utilities.Pool
{
    public delegate void PoolStateChangedEventHandler<T>(IPool<T> sender,T arg);
    
    public static partial class Cache<T> where T : class
    {
        public static readonly CacheRegister Caches;
        private static readonly string DefaultName = "Default";

        public static int Count { get { return DefaultPool.Count; } }
        public static IPool<T> DefaultPool { get; private set; }

        public static event PoolStateChangedEventHandler<T> Added = (sender, arg) => { }; 
        public static event PoolStateChangedEventHandler<T> Removed = (sender, arg) => { };
        public static event PoolStateChangedEventHandler<T> AddedAny = (sender, arg) => { };
        public static event PoolStateChangedEventHandler<T> RemovedAny = (sender, arg) => { };

        static Cache()
        {
            Caches = new CacheRegister();
            DefaultPool = Caches[DefaultName];
            DefaultPool.SetResetAction((instance) => { });
            DefaultPool.Added += (sender, arg) => Added(sender,arg);
            DefaultPool.Removed += (sender, arg) => Removed(sender,arg);
            CacheLog.AddType(typeof(Cache<T>));
        }

        public static void Generate(int count)
        {
            DefaultPool.Generate(count);
        }

        public static void SetFactory(Func<T> factoryMethod)
        {
            DefaultPool.SetFactory(factoryMethod);
        }

        public static void SetResetAction(Action<T> resetMethod)
        {
            DefaultPool.SetResetAction(resetMethod);
        }

        public static T Pop()
        {
            return DefaultPool.Pop();
        }

        public static T PopFrom(string cacheId)
        {
            return Caches[cacheId].Pop();
        }

        public static void Push(T instance)
        {
            DefaultPool.Push(instance);
        }

        public static void PushInto(T instance, string cacheId)
        {
            Caches[cacheId].Push(instance);
        }

        public static string GetTypeName()
        {
            return typeof(T).Name;
        }
    }
}
