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
    public class ObjectRegistry
    {
        // stores guid-object pairs of objects being serialized/deserialized as 2 dictionaries to facilitate preserving references.
        // has to be cleared before serialization / deserialization can proceed.

        Dictionary<Guid, object> guidToObj = new Dictionary<Guid, object>();
        Dictionary<object, Guid> objToGuid = new Dictionary<object, Guid>();

        /// <summary>
        /// Clears the registry.
        /// </summary>
        public void Clear()
        {
            guidToObj.Clear();
            objToGuid.Clear();
        }

        /// <summary>
        /// Registers the specific object with the specified GUID. Does nothing if the object is already registered with any GUID.
        /// </summary>
        public void TryRegister( Guid guid, object obj )
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
        public void Register( Guid guid, object obj )
        {
#warning TODO - Remove null entries? This is a memory management problem if it's not cleaned at some point.
            guidToObj.Add( guid, obj );
            objToGuid.Add( obj, guid );
        }

        /// <summary>
        /// Returns the object that's registered under the specified GUID. Throws exception if the guid is not registered.
        /// </summary>
        public object Get( Guid guid )
        {
            return guidToObj[guid];
        }

        /// <summary>
        /// Returns the GUID of the specific object.  Throws exception if the object is not registered.
        /// </summary>
        public Guid Get( object obj )
        {
            return objToGuid[obj];
        }
    }
}
