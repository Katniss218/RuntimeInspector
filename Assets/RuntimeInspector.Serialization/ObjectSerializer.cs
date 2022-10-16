using Newtonsoft.Json.Linq;
using RuntimeInspector.Core.AssetManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RuntimeInspector.Serialization
{
    /// <summary>
    /// A class for serializing data types.
    /// </summary>
    /// <remarks>
    /// A general data type can be serializes as either a JValue, a JArray, or a JObject.
    /// </remarks>
    public static partial class ObjectSerializer
    {
        private static ObjectRegistry objRegistry = new ObjectRegistry();

        /// <summary>
        /// The special token name for a System.Type.
        /// </summary>
        public const string TYPE = "$type";
        /// <summary>
        /// The special token name for a reference.
        /// </summary>
        public const string REF = "$ref";
        /// <summary>
        /// The special token name for an asset reference.
        /// </summary>
        public const string ASSETREF = "$assetref";

        /// <summary>
        /// Writes a Globally-Unique Identifier (GUID/UUID)
        /// </summary>
        public static JToken WriteGuid( Guid value )
        {
            return new JValue( value.ToString( "D" ) ); // D specifies '00000000-0000-0000-0000-000000000000' format.
        }

        /// <summary>
        /// Reads a Globally-Unique Identifier (GUID/UUID)
        /// </summary>
        public static Guid ReadGuid( JToken json )
        {
            return Guid.ParseExact( (string)json, "D" ); // D specifies '00000000-0000-0000-0000-000000000000' format.
        }

        /// <summary>
        /// Writes a delegate (reference to a method).
        /// </summary>
        /// <remarks>
        /// This is capable of fully serializing an arbitrary delegate, including lambdas and references to instance methods.
        /// 2. CHANGING CODE MIGHT INVALIDATE REFERENCES TO LAMBDAS.
        /// </remarks>
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

        /// <summary>
        /// Reads a delegate (reference to a method).
        /// </summary>
        /// <remarks>
        /// This is capable of fully deserializing an arbitrary delegate, including lambdas and references to instance methods.
        /// 1. THE TARGET OBJECT SHOULD BE DESERIALIZED BEFOREHAND.
        /// 2. CHANGING CODE MIGHT INVALIDATE REFERENCES TO LAMBDAS.
        /// </remarks>
        public static object ReadDelegate( JToken json )
        {
            // TODO - this requires the target to be already deserialized.
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

        private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        private static Dictionary<Type, string> typeCacheReversed = new Dictionary<Type, string>();

        public static JToken WriteType( Type value )
        {
            // 'AssemblyQualifiedName' is guaranteed to always uniquely identify a type.
            string assemblyQualifiedName;
            if( typeCacheReversed.TryGetValue( value, out assemblyQualifiedName ) )
            {
                return new JValue( assemblyQualifiedName );
            }

            // Cache the type because accessing the Type.AssemblyQualifiedName and Type.GetType(string) is very slow.
            assemblyQualifiedName = value.AssemblyQualifiedName;
            typeCache.Add( assemblyQualifiedName, value );
            typeCacheReversed.Add( value, assemblyQualifiedName );

            return new JValue( assemblyQualifiedName );
        }

        public static Type ReadType( JToken json )
        {
            // 'AssemblyQualifiedName' is guaranteed to always uniquely identify a type.
            Type type;
            string assemblyQualifiedName = (string)json;
            if( typeCache.TryGetValue( assemblyQualifiedName, out type ) )
            {
                return type;
            }

            // Cache the type because accessing the Type.AssemblyQualifiedName and Type.GetType(string) is very slow.
            type = Type.GetType( assemblyQualifiedName );
            typeCache.Add( assemblyQualifiedName, type );
            typeCacheReversed.Add( type, assemblyQualifiedName );

            return type;
        }

        public static JToken WriteObjectReference( object value )
        {
            // missing '$ref'  =>  null.

            if( value == null )
            {
                return new JObject();
            }

            Guid guid = Guid.NewGuid();
            objRegistry.TryRegister( guid, value );
            // Add the object to the registry if it's not already added.
            // This might lead to memory leaks if the registry is not cleared, old destroyed objects will linger as null/invalid references in the registry, and use up entries.
            return new JObject()
            {
                { $"{REF}", WriteGuid( guid) }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// REQUIRES THE OBJECT TO BE DESERIALIZED.
        /// </remarks>
        public static object ReadObjectReference( JToken json )
        {
            // missing '$ref'  =>  null.

            if( ((JObject)json).TryGetValue( $"{REF}", out JToken val ) )
            {
                Guid guid = ReadGuid( val );
                return objRegistry.Get( guid );
            }
            return null;
        }

        public static JToken WriteAssetReference<T>( T asset )
        {
            string assetID = AssetRegistry<T>.GetAssetID( asset );

            return new JObject()
            {
                { $"{ASSETREF}", assetID }
            };
        }

        public static T ReadAssetReference<T>( JToken json )
        {
            return AssetRegistry<T>.GetAsset( (string)json[$"{ASSETREF}"] );
        }

        /// <summary>
        /// Writes an empty value for any object, preserves the type of the instance.
        /// </summary>
        internal static JObject WriteTypedObject( object value )
        {
            return new JObject()
            {
                { $"{TYPE}", WriteType(value.GetType()) }
            };
        }

        /// <summary>
        /// Reads an empty value for any object, preserves the type of the instance.
        /// </summary>
        internal static object ReadTypedObject( JObject json )
        {
            Type type = ReadType( json[$"{TYPE}"] );
            object value = Activator.CreateInstance( type );
            return value;
        }

        /// <summary>
        /// Writes a any non-component class that implements ICustomSerializable to JSON.
        /// </summary>
        public static JToken WriteCustom<T>( T value ) where T : ISelfSerialize, new()
        {
            return value.WriteJson();
        }

        /// <summary>
        /// Reads a any non-component class that implements ICustomSerializable from JSON.
        /// </summary>
        public static T ReadCustom<T>( JToken json ) where T : ISelfSerialize, new()
        {
            // TODO - subclasses should construct the actual subclass.
            T value = new T();
            value.ReadJson( json );
            return value;
        }
    }
}