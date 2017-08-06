using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grygus.Utilities.Pool
{
    public class ObjectAlreadyInPoolException : System.Exception
    {
        public ObjectAlreadyInPoolException() { }
        public ObjectAlreadyInPoolException(Type cacheType, string cacheName)
            : this("Object is already in cached " + cacheType.Name + "|"+cacheName) { }
        public ObjectAlreadyInPoolException(string message) : base(message) { }
        public ObjectAlreadyInPoolException(string message, System.Exception inner) : base(message, inner) { }
        protected ObjectAlreadyInPoolException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
