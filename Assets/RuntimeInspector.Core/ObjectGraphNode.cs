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
    /// Represents a node in a tree of objects with fields, properties, etc.
    /// </summary>
    public abstract class ObjectGraphNode
    {
        /// <summary>
        /// The node that this node is a member of.
        /// </summary>
        public ObjectGraphNode Parent { get; private set; }

        bool _setMembersCalled = false;
        private List<ObjectGraphNode> _children = new List<ObjectGraphNode>();

        /// <summary>
        /// The attributes of this graph node.
        /// </summary>
        protected Attribute[] Attributes { get; set; }

        /// <summary>
        /// Checks whether the value is the root of the graph.
        /// </summary>
        public bool IsRoot => this.Parent == null;

        /// <summary>
        /// Reflected name of the member.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the class/struct that declared this member.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        /// The type of the member.
        /// </summary>
        /// <remarks>
        /// When reflecting an 'int' field, it will be equal to 'typeof( int )'.
        /// </remarks>
        public Type Type { get; }

        /// <summary>
        /// If true, this member can be read from (false for write-only).
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        /// If true, this member can be written to (false for read-only).
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// Returns the type of the currently assigned value, or the declared type of the member, if the value is not readable or null.
        /// </summary>
        public Type GetInstanceType()
        {
            if( !this.CanRead )
            {
                return this.Type;
            }

            object value = this.GetValue();
            if( value == null )
            {
                return this.Type;
            }

            return value.GetType();
        }

        /// <summary>
        /// Creates a new ObjectGraphNode and adds it to the hierarchy of its parent.
        /// </summary>
        protected ObjectGraphNode( ObjectGraphNode parent, string name, Type declaringType, Type type, bool canRead, bool canWrite )
        {
            this.SetParent( parent );

            this.Name = name;
            this.DeclaringType = declaringType;
            this.Type = type;
            this.CanRead = canRead;
            this.CanWrite = canWrite;
        }

        /// <summary>
        /// Adds the current graph node to the hierarchy of the specified graph node.
        /// </summary>
        internal void SetParent( ObjectGraphNode parent )
        {
            if( parent == this.Parent )
            {
                return;
            }

            // Remove outselves from our parent.
            if( this.Parent != null )
            {
                this.Parent._children.Remove( this );
            }

            // Set our parent.
            this.Parent = parent;

            // Add outselves to our new parent's children.
            if( parent != null )
            {
                foreach( var child in parent._children )
                {
                    if( child.Equals( this ) )
                    {
                        throw new InvalidOperationException( $"Can't set parent of {{{this}}} to {{{parent}}}. An equivalent node already exists on the parent." );
                    }
                }

                this.Parent._children.Add( this );
            }
        }

        /// <summary>
        /// The members that this value declares.
        /// </summary>
        public List<ObjectGraphNode> GetChildren()
        {
            TrySetMembers();
            return _children;
        }

        /// <summary>
        /// Returns a child node by its name and declaring type.
        /// </summary>
        public ObjectGraphNode GetChild( string name, Type declaringType )
        {
            foreach( var child in this.GetChildren() )
            {
                if( child.Name == name && child.DeclaringType == declaringType )
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all of the attributes of the specified type belonging to this graph node.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="includeDerived">If true it will include attributes derived from the target attribute, otherwise it will look for the exact type specified.</param>
        public List<T> GetAttributes<T>( bool includeDerived = true ) where T : Attribute
        {
            List<T> matchedAttribs = new List<T>();

            if( Attributes == null )
            {
                return matchedAttribs;
            }

            foreach( var attrib in Attributes )
            {
                if( includeDerived )
                {
                    if( attrib is T a )
                    {
                        matchedAttribs.Add( a );
                    }
                }
                else
                {
                    if( attrib.GetType() == typeof( T ) )
                    {
                        matchedAttribs.Add( (T)attrib );
                    }
                }
            }

            return matchedAttribs;
        }

        /// <summary>
        /// Returns the actual object value of this member.
        /// </summary>
        public abstract object GetValue();

        /// <summary>
        /// Sets the actual object value of this member.
        /// </summary>
        /// <remarks>
        /// Does not recalculate the graph. Take another snapshot to get an up-to-date graph.
        /// </remarks>
        public abstract void SetValue( object value );

        /// <summary>
        /// Figures out what fields/properties/etc are members of this specific graph node and adds them as children.
        /// </summary>
        protected void TrySetMembers()
        {
            if( _setMembersCalled )
            {
                return;
            }
            _setMembersCalled = true; // this needs to be at the start, or we get a stack overflow.

            Type instanceType = this.GetInstanceType();

            Type currentDeclaringType = instanceType;

            // Do not call this on a primitive type. They have self-referencing fields of the same type that never stop.
            if( currentDeclaringType.IsPrimitive )
            {
                return;
            }

            while( true )
            {
                FieldInfo[] fields = currentDeclaringType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
                PropertyInfo[] properties = currentDeclaringType.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

                MethodInfo[] methods = currentDeclaringType.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

                foreach( var field in fields )
                {
                    ObjectGraphNode member = new ObjectGraphNodeField( field, this );
                }

                foreach( var property in properties )
                {
                    ObjectGraphNode binding = new ObjectGraphNodeProperty( property, this );
                }

#warning TODO - methods and events.

                if( currentDeclaringType.BaseType == null )
                {
                    return;
                }
                currentDeclaringType = currentDeclaringType.BaseType;
            }
        }

        /// <summary>
        /// Checks whether the two GraphObjectNodes are the same node by equivalence.
        /// </summary>
        public override bool Equals( object obj )
        {
            if( obj is not ObjectGraphNode other )
            {
                return false;
            }

            return this.Name == other.Name
                && this.DeclaringType == other.DeclaringType
                && (this.Parent == null ? other.Parent == null : this.Parent.Equals( other.Parent ));
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{this.Type} {this.Name}";
        }

        /// <summary>
        /// Creates a new graph that points directly to an object.
        /// </summary>
        /// <remarks>
        /// The root object can't be reassigned, but its members can.
        /// </remarks>
        public static ObjectGraphNode CreateGraph( string graphName, object root )
        {
            return new ObjectGraphNodeDirect( graphName, root );
        }

        /// <summary>
        /// Creates a new graph with getters and setters to change the value of the root object.
        /// </summary>
        public static ObjectGraphNode CreateGraph( string graphName, Func<object> getter, Action<object> setter )
        {
            return new ObjectGraphNodeArbitrary( graphName, getter, setter );
        }

        /// <summary>
        /// Adds a new arbitrary node to the graph.
        /// </summary>
        public ObjectGraphNode AddNode( string name, Type declaringType, Func<object> getter, Action<object> setter )
        {
            return new ObjectGraphNodeArbitrary( this, name, declaringType, getter, setter );
        }
    }
}