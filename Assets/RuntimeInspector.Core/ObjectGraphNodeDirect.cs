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
    /// Represents a graph node that directly references an object (value).
    /// </summary>
    /// <remarks>
    /// This node must be a root node.
    /// </remarks>
    public sealed class ObjectGraphNodeDirect : ObjectGraphNode
    {
        private object _backingObject;

        internal ObjectGraphNodeDirect( object root )
            : base( null, null, null, root.GetType(), true, true )
        {
        }

        public override object GetValue()
        {
            return _backingObject;
        }

        public override void SetValue( object value )
        {
            Debug.LogWarning( "Tried SetValue on ObjectGraphNull." );
            _backingObject = value;
        }
    }
}