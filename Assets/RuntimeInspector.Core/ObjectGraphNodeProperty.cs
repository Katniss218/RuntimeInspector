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

        public Type[] IndexParameters { get; set; } = null;

        internal ObjectGraphNodeProperty( PropertyInfo property, ObjectGraphNode parent )
            : base( parent, property.Name, property.DeclaringType, property.PropertyType, property.CanRead, property.CanWrite )
        {
            this._property = property;
            this._backingObject = parent.GetValue();

            this.Attributes = this._property.GetCustomAttributes().ToArray();
            this.IndexParameters = this._property.GetIndexParameters().Select( p => p.ParameterType ).ToArray();
        }

        public override bool Equals( object obj )
        {
            if( obj is not ObjectGraphNodeProperty other )
            {
                return false;
            }

            return base.Equals( obj )
                && (IndexParameters == null ? other.IndexParameters == null : IndexParameters.SequenceEqual( other.IndexParameters ));
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
        /*
        public object GetValue( params object[] indices )
        {
            if( !CanRead )
            {
                Debug.LogWarning( "Tried to get indexed value of a write-only property." );
                return null;
            }

            return _property.GetValue( _backingObject, indices );
        }
        */
        public override void SetValue( object value )
        {
            if( !CanWrite )
            {
                Debug.LogWarning( "Tried to set value of a read-only property." );
                return;
            }

            _property.SetValue( _backingObject, value );
        }
        /*
        public void SetValue( object value, params object[] indices )
        {
            if( !CanWrite )
            {
                Debug.LogWarning( "Tried to set indexed value of a read-only property." );
                return;
            }

            _property.SetValue( _backingObject, value, indices );
        }*/
    }
}