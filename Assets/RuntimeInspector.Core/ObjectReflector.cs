using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{/*
    public class ObjectReflector
    {
        // we need an object to bind the edited object(s) to.

        private object instance;
        private Type objType;

        public IMemberBinding ReflectedBinding { get; private set; }

        public void BindTo( object instance )
        {
            Type currentType = instance.GetType();

            this.instance = instance;
            this.objType = currentType;

            /*
            foreach( var method in methods )
            {
                objMethods.Add( method.Name, method );
            }

            foreach( var @event in events )
            {
                objEvents.Add( @event.Name, @event );
            }*
        }

        private IMemberBinding GetBindingInfo( object instance )
        {
            IMemberBinding reflectedBinding = new List<TypeInfo>();

            while( true )
            {
               
                FieldBinding[] fieldBindings = fields.Select( f => FieldBinding.GetBinding( f, instance ) ).ToArray();
                PropertyBinding[] propertyBindings = properties.Select( p => PropertyBinding.GetBinding( p, instance ) ).ToArray();

                ReflectedInfo.Add( new TypeInfo()
                {
                    DeclaringType = currentType,
                    Fields = fieldBindings,
                    Properties = propertyBindings
                } );

                // break out of the loop when the root type is found.
                currentType = currentType.BaseType;
                if( currentType == null )
                {
                    break;
                }
            }
        }

        public void Unbind()
        {
            instance = null;
            objType = null;
            ReflectedInfo = null;
        }

        public bool IsBound => instance != null;

        /*
        //      CALLABLES

        public object CallField( string name, params object[] parameters )
        {
            object field = objFields[name].GetValue( obj );
            MethodInfo invokeMethod = field.GetType().GetMethod( "Invoke" ); // Gets the "Invoke" method on the delegate object assigned to the field.

            return invokeMethod.Invoke( field, parameters );
        }

        public object CallProperty( string name, params object[] parameters )
        {
            object property = objProperties[name].GetValue( obj );
            MethodInfo invokeMethod = property.GetType().GetMethod( "Invoke" ); // Gets the "Invoke" method on the delegate object assigned to the field.

            return invokeMethod.Invoke( property, parameters );
        }

        public object CallMethod( string name, params object[] parameters )
        {
            return objMethods[name].Invoke( obj, parameters );
        }*
    }*/
}