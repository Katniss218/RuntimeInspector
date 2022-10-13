using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public class ObjectReflector
    {
        // we need an object to bind the edited object(s) to.

        private object instance;
        private Type objType;

        public IMemberBinding[] AssignableMembers;

        public void BindTo( object instance )
        {
            this.instance = instance;
            this.objType = instance.GetType();

            FieldInfo[] fields = objType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            PropertyInfo[] properties = objType.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

            //MethodInfo[] methods = objType.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            //EventInfo[] events = objType.GetEvents( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

            List<IMemberBinding> members = new List<IMemberBinding>( fields.Length + properties.Length );

            foreach( var field in fields )
            {
                members.Add( new FieldBinding( field, instance ) );
            }
            foreach( var property in properties )
            {
                members.Add( new PropertyBinding( property, instance ) );
            }

            AssignableMembers = members.ToArray();
            /*
            foreach( var method in methods )
            {
                objMethods.Add( method.Name, method );
            }

            foreach( var @event in events )
            {
                objEvents.Add( @event.Name, @event );
            }*/
        }

        public void Unbind()
        {
            instance = null;
            objType = null;
            AssignableMembers = null;
        }

        public bool IsBound => instance != null;
        /*
        //      FIELDS

        public object GetField( string name )
        {
            return objFields[name].GetValue( obj );
        }

        public void SetField( string name, object value )
        {
            objFields[name].SetValue( obj, value );
        }

        public void SetField( string name, object instance, string methodName )
        {
            Type delegateType = objFields[name].GetValue( obj ).GetType(); // TODO - cacheable.
            MethodInfo targetMethod = instance.GetType().GetMethod( methodName );
            Delegate del = Delegate.CreateDelegate( delegateType, instance, targetMethod );
            objFields[name].SetValue( obj, del );
        }

        //      PROPERTIES

        public object GetProperty( string name )
        {
            return objProperties[name].GetValue( obj );
        }

        public void SetProperty( string name, object value )
        {
            objProperties[name].SetValue( obj, value );
        }

        public void SetProperty( string name, object instance, string methodName )
        {
            Type delegateType = objProperties[name].GetValue( obj ).GetType(); // TODO - cacheable.
            MethodInfo targetMethod = instance.GetType().GetMethod( methodName );
            Delegate del = Delegate.CreateDelegate( delegateType, instance, targetMethod );
            objProperties[name].SetValue( obj, del );
        }

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
        }*/
    }
}