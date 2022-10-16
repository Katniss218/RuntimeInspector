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
    /// A class for serializing gameobjects and their constituents.
    /// </summary>
    /// <remarks>
    /// Every Component must be serialized as a JObject.
    /// </remarks>
    public static partial class ComponentSerializer
    {
        public static JObject WriteTransform( Transform component )
        {
            return new JObject()
            {
                { "Position", ObjectSerializer.WriteVector3( component.position ) },
                { "Rotation", ObjectSerializer.WriteQuaternion( component.rotation ) },
                { "Scale", ObjectSerializer.WriteVector3( component.localScale ) }
            };
        }

        public static Transform ReadTransform( GameObject gameObject, JObject json )
        {
            Transform component = gameObject.transform;

            component.position = ObjectSerializer.ReadVector3( json["Position"] );
            component.rotation = ObjectSerializer.ReadQuaternion( json["Rotation"] );
            component.localScale = ObjectSerializer.ReadVector3( json["Scale"] );

            return component;
        }

        public static JObject WriteMeshFilter( MeshFilter component )
        {
            return new JObject()
            {
                { "Mesh", ObjectSerializer.WriteAssetReference( component.mesh ) },
            };
        }

        public static MeshFilter ReadMeshFilter( GameObject gameObject, JObject json )
        {
            MeshFilter component = gameObject.AddComponent<MeshFilter>();

            component.mesh = ObjectSerializer.ReadAssetReference<Mesh>( json["Mesh"] );

            return component;
        }

        public static JObject WriteMeshRenderer( MeshRenderer component )
        {
            JArray sharedMaterialsJson = new JArray();
            foreach( var mat in component.sharedMaterials )
            {
                sharedMaterialsJson.Add( ObjectSerializer.WriteAssetReference( mat ) );
            }

            /* TODO - this would have to serialize the material itself, not a reference to an asset.
            JArray materialsJson = new JArray();
            foreach( var mat in component.materials )
            {
                sharedMaterialsJson.Add( ObjectSerializer.WriteAssetReference( mat ) );
            }
            */

            return new JObject()
            {
                //{ "Materials", materialsJson },
                { "SharedMaterials", sharedMaterialsJson },
                { "ShadowCastingMode", component.shadowCastingMode.ToString() },
                { "ReceiveShadows", component.receiveShadows }
                // more if needed.
            };
        }

        public static MeshRenderer ReadMeshRenderer( GameObject gameObject, JObject json )
        {
            MeshRenderer component = gameObject.AddComponent<MeshRenderer>();

            List<Material> mats = new List<Material>();
            foreach( var sharedMatJson in json["SharedMaterials"] )
            
            {
                Material mat = ObjectSerializer.ReadAssetReference<Material>( sharedMatJson );
                mats.Add( mat );
            }

            component.sharedMaterials = mats.ToArray();
            component.shadowCastingMode = Enum.Parse<UnityEngine.Rendering.ShadowCastingMode>( (string)json["ShadowCastingMode"] );
            component.receiveShadows = (bool)json["ReceiveShadows"];

            return component;
        }

        public static JObject WriteCustom( ISelfSerialize component )
        {
            JObject json = new JObject()
            {
                { $"{ObjectSerializer.TYPE}", ObjectSerializer.WriteType(component.GetType()) },
                { "data", component.WriteJson() }
            };

            return json;
        }

        public static ISelfSerialize ReadCustom( GameObject gameObject, JObject json )
        {
            Type type = ObjectSerializer.ReadType( json[$"{ObjectSerializer.TYPE}"] );
            if( !typeof( ISelfSerialize ).IsAssignableFrom( type ) || !typeof( Component ).IsAssignableFrom( type ) ) // The user can pass us a JSON of a different, incompatible object. Might be worth cache'ing.
            {
                throw new InvalidOperationException( $"The {type} isn't a serializable component." );
            }

            ISelfSerialize component = (ISelfSerialize)gameObject.AddComponent( type );

            component.ReadJson( json["data"] );

            return component;
        }

        public static JObject WriteGameObject( GameObject gameObject )
        {
            // gameobjects are serialized as a tree hierarchy.
            // Every component is serialized with the corresponding gameobject, not in the reference pool.

            JArray childrenJson = new JArray();
            for( int i = 0; i < gameObject.transform.childCount; i++ )
            {
                GameObject child = gameObject.transform.GetChild( i ).gameObject;
                childrenJson.Add( WriteGameObject( child ) );
            }

            JArray componentsJson = new JArray();
            foreach( var component in gameObject.GetComponents<Component>() ) // This doesn't return the UnityEngine.Transform, despite it being derived from UnityEngine.Component.
            {
                if( component is ISelfSerialize )
                {
                    componentsJson.Add( WriteCustom( (ISelfSerialize)component ) );
                    continue;
                }

                JObject componentJson = new JObject()
                {
                    { $"{ObjectSerializer.TYPE}", ObjectSerializer.WriteType( component.GetType() ) },
                };
                if( component is MeshFilter )
                {
                    componentJson["data"] = WriteMeshFilter( (MeshFilter)component );
                    componentsJson.Add( componentJson );
                    continue;
                }
                if( component is MeshRenderer )
                {
                    componentJson["data"] = WriteMeshRenderer( (MeshRenderer)component );
                    componentsJson.Add( componentJson );
                    continue;
                }
            }

            JObject json = new JObject()
            {
                { "Transform", WriteTransform(gameObject.transform) },
                { "Name", gameObject.name },
                { "Tag", gameObject.tag },
                { "IsActive", gameObject.activeSelf },
                { "IsStatic", gameObject.isStatic },
                { "Layer", gameObject.layer },
                { "__children", childrenJson },
                { "Components", componentsJson }
            };
            return json;
        }

        public static GameObject ReadGameObject( JObject json )
        {
            GameObject go = new GameObject();
            go.name = (string)json["Name"];
            go.tag = (string)json["Tag"];
            go.SetActive( (bool)json["IsActive"] );
            go.isStatic = (bool)json["IsStatic"];
            go.layer = (int)json["Layer"];

            ReadTransform( go, (JObject)json["Transform"] );

            foreach( JObject componentJson in json["Components"] ) // This doesn't return the UnityEngine.Transform, despite it being derived from UnityEngine.Component.
            {
                Type type = ObjectSerializer.ReadType( componentJson[$"{ObjectSerializer.TYPE}"] );

                if( typeof( ISelfSerialize ).IsAssignableFrom(type) )
                {
                    ReadCustom( go, componentJson );
                    continue;
                }

                if( type == typeof( MeshFilter ) )
                {
                    ReadMeshFilter( go, (JObject)componentJson["data"] );
                    continue;
                }
                if( type == typeof( MeshRenderer ) )
                {
                    ReadMeshRenderer( go, (JObject)componentJson["data"] );
                    continue;
                }
            }

            return go;
        }
    }
}