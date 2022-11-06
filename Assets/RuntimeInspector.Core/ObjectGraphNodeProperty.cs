using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Represents a graph node that is a property of the parent node.
    /// </summary>
    /// <remarks>
    /// This node can not be a root node.
    /// </remarks>
    public sealed class ObjectGraphNodeProperty : ObjectGraphNode
    {
        private PropertyInfo _property;
        private object _backingObject;
        
        internal ObjectGraphNodeProperty( PropertyInfo property, ObjectGraphNode parent )
            : base( parent, property.Name, property.DeclaringType, property.PropertyType, property.CanRead, property.CanWrite )
        {
            this._property = property;
            this._backingObject = parent.GetValue();
#warning TODO - stop getting fields of null objects. Parent.GetValue() [Parent was BaseClass.MeshFilter] returned null.

            this.Attributes = this._property.GetCustomAttributes().ToArray();
        }

        public override object GetValue()
        {
            if( !CanRead )
            {
                Debug.LogWarning( "Tried to get value of a write-only property." );
                return null;
            }

            return _property.GetValue( _backingObject );
        }

        public override void SetValue( object value )
        {
            if( !CanWrite )
            {
                Debug.LogWarning( "Tried to set value of a read-only property." );
                return;
            }

            _property.SetValue( _backingObject, value );
        }
    }
}