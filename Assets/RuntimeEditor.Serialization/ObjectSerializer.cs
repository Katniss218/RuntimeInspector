using Newtonsoft.Json.Linq;
using UnityPlus.AssetManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RuntimeEditor.Serialization
{
    /// <summary>
    /// A class for serializing data types.
    /// </summary>
    /// <remarks>
    /// A general data type can be serializes as either a JValue, a JArray, or a JObject.
    /// </remarks>
    public static partial class ObjectSerializer
    {
        /// <summary>
        /// The special token name for a System.Type.
        /// </summary>
        public const string TYPE = "$type";
        /// <summary>
        /// The special token name for a reference (part of Reference).
        /// </summary>
        public const string REF = "$ref";
        /// <summary>
        /// The special token name for an asset reference.
        /// </summary>
        public const string ASSETREF = "$assetref";

        /// <summary>
        /// The special token name for a reference ID (part of Object).
        /// </summary>
        public const string ID = "$id";

        public const string VALUE = "value";

        public static void StartSerialization()
        {
            ObjectRegistry.Clear();
        }

        public static void EndSerialization()
        {
            ObjectRegistry.Clear();
        }

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
        /// This is capable of fully serializing an arbitrary delegate, including multicasting, lambdas, and references to instance methods.
        /// 2. CHANGING CODE MIGHT INVALIDATE REFERENCES TO LAMBDAS.
        /// </remarks>
        public static JToken WriteDelegate( Delegate delegateObj )
        {
            JArray invocationListJson = new JArray();

            foreach( var del in delegateObj.GetInvocationList() )
            {
                JToken delJson = WriteSingleDelegate( del );
                invocationListJson.Add( delJson );
            }

            return invocationListJson;
        }

        private static JToken WriteSingleDelegate( Delegate delegateObj )
        {
            Type delegateType = delegateObj.GetType();

            MethodInfo method = delegateObj.Method;
            Type declaringType = method.DeclaringType;
            object target = delegateObj.Target;

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
        /// This is capable of fully deserializing an arbitrary delegate, including multicasting, lambdas, and references to instance methods.
        /// 1. THE TARGET OBJECT SHOULD BE DESERIALIZED BEFOREHAND.
        /// 2. CHANGING CODE MIGHT INVALIDATE REFERENCES TO LAMBDAS.
        /// </remarks>
        public static Delegate ReadDelegate( JToken json )
        {
            JArray jsonA = (JArray)json;

            if( jsonA.Count == 1 )
            {
                return ReadSingleDelegate( jsonA[0] );
            }

            Delegate[] invocationList = new Delegate[jsonA.Count];
            for( int i = 0; i < jsonA.Count; i++ )
            {
                invocationList[i] = ReadSingleDelegate( jsonA[i] );
            }
            return Delegate.Combine( invocationList );
        }

        private static Delegate ReadSingleDelegate( JToken json )
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
            Delegate delegateObj = method.CreateDelegate( delegateType, target );

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
            // A missing '$ref' node equals null.

            if( value == null )
            {
                return new JObject();
            }

            // If the object wasn't already serialized - generate a Guid for it, and add to the registry so we don't generate a different guid later.
            if( !ObjectRegistry.TryGet( value, out Guid guid ) )
            {
                guid = Guid.NewGuid();
                ObjectRegistry.Register( guid, value );
            }

            return new JObject()
            {
                { $"{REF}", WriteGuid( guid) }
            };
        }

        public static object ReadObjectReference( JToken json )
        {
            // A missing '$ref' node equals null.

            if( ((JObject)json).TryGetValue( $"{REF}", out JToken refJson ) )
            {
                Guid guid = ReadGuid( refJson );
                if( ObjectRegistry.TryGet( guid, out object value ) )
                {
                    return value;
                }
                // TODO - load the object.
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
        /*
        /// <summary>
        /// Writes an ISelfSerialize object, preserves the inheritance tree of the instance.
        /// </summary>
        internal static JObject WriteObject( ISelfSerialize value )
        {
            return new JObject()
            {
                { $"{TYPE}", WriteType(value.GetType()) },
                { "{VALUE}", value.WriteJson() }
            };
        }
        #error TODO - this should work with the ObjectRegistry.
        /// <summary>
        /// Reads an ISelfSerialize object, preserves the inheritance tree of the instance.
        /// </summary>
        internal static ISelfSerialize ReadObject( JObject json )
        {
            Type type = ReadType( json[$"{TYPE}"] );
            ISelfSerialize value = (ISelfSerialize)Activator.CreateInstance( type );
            value.ReadJson( json[$"{VALUE}"] );
            return value;
        }*/
    }
}