using Newtonsoft.Json.Linq;
using RuntimeInspector.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.Serialization
{
    /// <summary>
    /// Supports serializing and deserializing gameobjects and their constituents
    /// </summary>
    public static partial class ComponentSerializer
    {
        public static JToken WriteTransform( Transform component )
        {
            return new JObject()
            {
                { "Position", ObjectSerializer.WriteVector3( component.position ) },
                { "Rotation", ObjectSerializer.WriteQuaternion( component.rotation ) },
                { "Scale", ObjectSerializer.WriteVector3( component.localScale ) }
            };
        }

        public static Transform ReadTransform( GameObject gameObject, JToken json )
        {
            Transform component = gameObject.transform;

            component.position = ObjectSerializer.ReadVector3( json["Position"] );
            component.rotation = ObjectSerializer.ReadQuaternion( json["Rotation"] );
            component.localScale = ObjectSerializer.ReadVector3( json["Scale"] );

            return component;
        }

        public static JToken WriteMeshFilter( MeshFilter component )
        {
            return new JObject()
            {
                { "Mesh", ObjectSerializer.WriteAssetReference( component.mesh ) },
            };
        }

        public static MeshFilter ReadMeshFilter( GameObject gameObject, JToken json )
        {
            MeshFilter component = gameObject.AddComponent<MeshFilter>();

            component.mesh = ObjectSerializer.ReadAssetReference<Mesh>( json["Mesh"] );

            return component;
        }

        public static JToken WriteCustom<T>( T component ) where T : Component, ICustomSerializer
        {
            return component.WriteJson();
        }

        public static T ReadCustom<T>( GameObject gameObject, JToken json ) where T : Component, ICustomSerializer
        {
            T component = gameObject.AddComponent<T>(); // TODO - subclasses

            component.PopulateJson( json );

            return component;
        }
    }
}
