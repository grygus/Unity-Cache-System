using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public class NoObjectsInPoolException : System.Exception
    {
        public NoObjectsInPoolException() { }
        public NoObjectsInPoolException(Type cacheType, string cacheName)
            : this("No objects left in cache " + cacheType.Name + "|"+cacheName) { }
        public NoObjectsInPoolException(string message) : base(message) { }
        public NoObjectsInPoolException(string message, System.Exception inner) : base(message, inner) { }
        protected NoObjectsInPoolException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
