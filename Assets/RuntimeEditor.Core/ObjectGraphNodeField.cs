using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeEditor.Core
{
    /// <summary>
    /// Represents a graph node that is a field of the parent node.
    /// </summary>
    /// <remarks>
    /// This node can not be a root node.
    /// </remarks>
    public sealed class ObjectGraphNodeField : ObjectGraphNode
    {
        private FieldInfo _field;
        private object _backingObject;

        /// <summary>
        /// Creates a graph member from a field of an object.
        /// </summary>
        internal ObjectGraphNodeField( FieldInfo field, ObjectGraphNode parent )
            : base( parent, field.Name, field.DeclaringType, field.FieldType, true, true )
        {
            this._field = field;
            this._backingObject = parent.GetValue();

            this.Attributes = this._field.GetCustomAttributes().ToArray();
        }

        public override object GetValue()
        {
            return _field.GetValue( _backingObject );
        }

        public override void SetValue( object value )
        {
            _field.SetValue( _backingObject, value );
        }
    }
}