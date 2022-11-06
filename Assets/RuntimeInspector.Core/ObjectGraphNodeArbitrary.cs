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
    /// Represents a graph node for an object with arbitrarily defined getter and setter methods. Either or both methods can be null.
    /// </summary>
    /// <remarks>
    /// This node must be a root node.
    /// </remarks>
    public sealed class ObjectGraphNodeArbitrary : ObjectGraphNode
    {
        private Func<object> _getter;
        private Action<object> _setter;

        internal ObjectGraphNodeArbitrary( Func<object> getter, Action<object> setter )
            : base( null, null, null, getter()?.GetType(), getter != null, setter != null )
        {
            _getter = getter;
            _setter = setter;
        }

        public override object GetValue()
        {
            return _getter();
        }

        public override void SetValue( object value )
        {
            _setter( value );
        }
    }
}