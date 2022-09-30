using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RuntimeInspector.Serialization
{
    public class ObjectSerializer
    {
        public const string TYPE = "$type";
        public const string REF = "$ref";


        public static JToken WriteBool( bool value )
        {
            return new JValue( value );
        }

        public static bool ReadBool( JToken json )
        {
            return (bool)json;
        }

        public static JToken WriteGuid( Guid value )
        {
            return new JValue( value.ToString( "D" ) );
        }

        public static Guid ReadGuid( JToken json )
        {
            return Guid.ParseExact( (string)json, "D" );
        }

        public static JToken WriteType( Type value )
        {
            return new JValue( value.AssemblyQualifiedName );
        }

        public static Type ReadType( JToken json )
        {
            return Type.GetType( (string)json );
        }

        public static JToken WriteObjectReference( object value )
        {
            if( value == null )
            {
                return new JObject();
            }
            Guid guid = Guid.NewGuid();
            ObjectRegistry.Register( guid, value );
            return new JObject()
            {
                { $"{REF}", WriteGuid( guid) }
            };
        }

        public static object ReadObjectReference( JToken json )
        {
            if( ((JObject)json).TryGetValue( $"{REF}", out JToken val ) )
            {
                Guid guid = ReadGuid( val );
                return ObjectRegistry.Get(guid);
            }
            return null;
        }

        // write and read reference

        public static JToken WriteDelegate( object delegateObj )
        {
            Type delegateType = delegateObj.GetType();

            dynamic del = delegateObj;

            MethodInfo method = del.Method;
            Type declaringType = method.DeclaringType;
            object target = del.Target;

            JArray jsonParameters = new JArray();
            ParameterInfo[] parameters = method.GetParameters();
            foreach( var param in parameters )
            {
                jsonParameters.Add( param.ParameterType.AssemblyQualifiedName );
            }

            JObject obj = new JObject()
            {
                { "Method", new JObject() {
                    { "DelegateType", WriteType(delegateType) },
                    { "Identifier", method.Name },
                    { "Parameters", jsonParameters },
                    { "DeclaringType", WriteType(declaringType) }
                } },
                { "Target", WriteObjectReference(target) }
            };
            return obj;
        }

        public static object ReadDelegate( JToken json )
        {
            object target = ReadObjectReference( json["Target"] );

            Type delegateType = ReadType( json["Method"]["DelegateType"] );
            Type declaringType = ReadType( json["Method"]["DeclaringType"] );
            List<Type> parameters = new List<Type>();
            foreach( var jsonParam in json["Method"]["Parameters"] )
            {
                parameters.Add( ReadType( jsonParam ) );
            }
            string methodName = (string)json["Method"]["Identifier"];

            MethodInfo method = declaringType.GetMethod( methodName, parameters.ToArray() );
            object delegateObj = method.CreateDelegate( delegateType, target );

            return delegateObj;
            // returns the delegate object that is ready to be assigned to the field
        }
    }
}