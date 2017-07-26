using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public delegate void CacheCahangedEventHandler(object sender,object args);
    public static class CacheLog
    {
        public static event CacheCahangedEventHandler AnyAdded;
        public static event CacheCahangedEventHandler AnyRemoved;
        public static Dictionary<Type, int> CacheCounter;
        private static List<Type> _cacheTypes = new List<Type>();

        static CacheLog()
        {
            CacheCounter = new Dictionary<Type, int>();
            AnyAdded += (sender, args) =>
            {
                var argumentType = sender.GetType().GetGenericArguments()[0];
                CacheCounter[argumentType]++;
            };
            AnyRemoved += (sender, args) =>
            {
                var argumentType = sender.GetType().GetGenericArguments()[0];
                CacheCounter[argumentType]--;
            };

        }

        public static List<Type> GetTypes()
        {
            var result = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(Cache<>)
                            );
            return new List<Type>(result);

        }

        public static void AddType(Type cacheType)
        {
            CacheCounter[cacheType.GetGenericArguments()[0]] = 0;
            _cacheTypes.Add(cacheType);
            var eventInfo = cacheType.GetEvent("AddedAny");
            SubscriveToEvent(eventInfo, null, "OnAnyAdded");
            var removedEventInfo = cacheType.GetEvent("RemovedAny");
            SubscriveToEvent(removedEventInfo, null, "OnAnyRemoved");
        }

        public static List<Type> GetCacheClosedTypes()
        {
            return _cacheTypes;
        }

        private static void SubscriveToEvent(EventInfo eventInfo, object targetObject, string targetMethod)
        {
            ConstructorInfo constructor = eventInfo.EventHandlerType.GetConstructors()[0];
            Delegate handler = (Delegate)constructor.Invoke(new object[]
                   {
                    null,
                    typeof(CacheLog).GetMethod(targetMethod, BindingFlags.Static | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer()
                   });
            eventInfo.AddEventHandler(null, handler);
        }

        private static void OnAnyAdded(object sernder, object args)
        {
            AnyAdded(sernder, args);
        }

        private static void OnAnyRemoved(object sernder, object args)
        {
            AnyRemoved(sernder, args);
        }
    }
}
