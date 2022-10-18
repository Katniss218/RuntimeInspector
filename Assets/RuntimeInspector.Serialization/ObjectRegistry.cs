using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Serialization
{
    /// <summary>
    /// A class used for storing object metadata used for serialization.
    /// </summary>
    public static class ObjectRegistry
    {
        // stores guid-object pairs of objects being serialized/deserialized as 2 dictionaries to facilitate preserving references.
        // has to be cleared before serialization / deserialization can proceed.

        static Dictionary<Guid, object> guidToObj = new Dictionary<Guid, object>();
        static Dictionary<object, Guid> objToGuid = new Dictionary<object, Guid>();

        /// <summary>
        /// Clears the registry.
        /// </summary>
        public static void Clear()
        {
            guidToObj.Clear();
            objToGuid.Clear();
        }

        /// <summary>
        /// Registers the specific object with the specified GUID. Does nothing if the object is already registered with any GUID.
        /// </summary>
        public static void TryRegister( Guid guid, object obj )
        {
            if( guidToObj.ContainsKey( guid ) )
            {
                return;
            }
            guidToObj.Add( guid, obj );
            objToGuid.Add( obj, guid );
        }

        /// <summary>
        /// Registers the specific object with the specified GUID. Throws exception if the object or the GUID is already registered.
        /// </summary>
        public static void Register( Guid guid, object obj )
        {
            guidToObj.Add( guid, obj );
            objToGuid.Add( obj, guid );
        }

        /// <summary>
        /// Returns the object that's registered under the specified GUID. Throws exception if the guid is not registered.
        /// </summary>
        public static object Get( Guid guid )
        {
            return guidToObj[guid];
        }

        public static bool TryGet( Guid guid, out object obj )
        {
            return guidToObj.TryGetValue( guid, out obj );
        }

        /// <summary>
        /// Returns the GUID of the specific object.  Throws exception if the object is not registered.
        /// </summary>
        public static Guid Get( object obj )
        {
            return objToGuid[obj];
        }

        public static bool TryGet( object obj, out Guid guid )
        {
            return objToGuid.TryGetValue( obj, out guid );
        }
    }
}
