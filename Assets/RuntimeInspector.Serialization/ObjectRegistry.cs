using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Serialization
{
    public class ObjectRegistry
    {
        // stores guid-object pairs of objects being serialized/deserialized as 2 dictionaries to facilitate preserving references.
        // has to be cleared before serialization / deserialization can proceed.

        public static Dictionary<Guid, object> guidToObj = new Dictionary<Guid, object>();
        public static Dictionary<object, Guid> objToGuid = new Dictionary<object, Guid>();

        public static void Clear()
        {
            guidToObj.Clear();
            objToGuid.Clear();
        }

        public static void Add( Guid guid, object obj )
        {
            guidToObj.Add( guid, obj );
            objToGuid.Add( obj, guid );
        }
    }
}
