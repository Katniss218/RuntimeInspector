using Newtonsoft.Json.Linq;
using UnityPlus.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeEditor.Serialization
{
    public static class MeshFilterEx
    {
        public static JObject WriteJson( this MeshFilter component )
        {
            return new JObject()
            {
                { "Mesh", ObjectSerializer.WriteAssetReference( component.mesh ) },
            };
        }

        public static void ReadJson( this MeshFilter component, JObject json )
        {
            component.mesh = ObjectSerializer.ReadAssetReference<Mesh>( json["Mesh"] );
        }
    }

    public static class MeshRendererEx
    {
        public static JObject WriteJson( this MeshRenderer component )
        {
            JArray sharedMaterialsJson = new JArray();
            foreach( var mat in component.sharedMaterials )
            {
                sharedMaterialsJson.Add( ObjectSerializer.WriteAssetReference( mat ) );
            }

            /* TODO - this would have to serialize the material itself???, not a reference to an asset.
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

        public static void ReadJson( this MeshRenderer component, JObject json )
        {
            List<Material> mats = new List<Material>();
            foreach( var sharedMatJson in json["SharedMaterials"] )
            {
                Material mat = ObjectSerializer.ReadAssetReference<Material>( sharedMatJson );
                mats.Add( mat );
            }

            component.sharedMaterials = mats.ToArray();
            component.shadowCastingMode = Enum.Parse<UnityEngine.Rendering.ShadowCastingMode>( (string)json["ShadowCastingMode"] );
            component.receiveShadows = (bool)json["ReceiveShadows"];
        }
    }

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
            if( !ObjectRegistry.TryGet( component, out Guid guid ) )
            {
                guid = Guid.NewGuid();
                ObjectRegistry.Register( guid, component );
            }

            return new JObject()
            {
                { $"{ObjectSerializer.ID}", ObjectSerializer.WriteGuid(guid) },
                { "Position", ObjectSerializer.WriteVector3( component.localPosition ) },
                { "Rotation", ObjectSerializer.WriteQuaternion( component.localRotation ) },
                { "Scale", ObjectSerializer.WriteVector3( component.localScale ) }
            };
        }

        public static Transform ReadTransform( GameObject gameObject, JObject json )
        {
            Transform component = gameObject.transform;

            Guid guid = ObjectSerializer.ReadGuid( json[$"{ObjectSerializer.ID}"] );
            ObjectRegistry.Register( guid, component );

            component.localPosition = ObjectSerializer.ReadVector3( json["Position"] );
            component.localRotation = ObjectSerializer.ReadQuaternion( json["Rotation"] );
            component.localScale = ObjectSerializer.ReadVector3( json["Scale"] );

            return component;
        }

        public static JObject WriteCustom( ISelfSerialize component )
        {
            if( !ObjectRegistry.TryGet( component, out Guid guid ) )
            {
                guid = Guid.NewGuid();
                ObjectRegistry.Register( guid, component );
            }

            JObject json = new JObject()
            {
                { $"{ObjectSerializer.TYPE}", ObjectSerializer.WriteType(component.GetType()) },
                { $"{ObjectSerializer.ID}", ObjectSerializer.WriteGuid(guid) },
                { $"{ObjectSerializer.VALUE}", component.WriteJson() }
            };

            return json;
        }

        public static ISelfSerialize ReadCustomPart1( GameObject gameObject, JObject json )
        {
            Type type = ObjectSerializer.ReadType( json[$"{ObjectSerializer.TYPE}"] );
            if( !typeof( ISelfSerialize ).IsAssignableFrom( type ) || !typeof( Component ).IsAssignableFrom( type ) ) // The user can pass us a JSON of a different, incompatible object. Might be worth cache'ing.
            {
                throw new InvalidOperationException( $"The {type} isn't a serializable component." );
            }

            ISelfSerialize component = (ISelfSerialize)gameObject.AddComponent( type );

            Guid guid = ObjectSerializer.ReadGuid( json[$"{ObjectSerializer.ID}"] );
            ObjectRegistry.Register( guid, component );

            //component.ReadJson( json["{ObjectSerializer.VALUE}"] ); -- this runs later after all components have been loaded.

            return component;
        }

        public static JObject WriteGameObject( GameObject gameObject )
        {
            if( !ObjectRegistry.TryGet( gameObject, out Guid guid ) )
            {
                guid = Guid.NewGuid();
                ObjectRegistry.Register( guid, gameObject );
            }

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

                if( !ObjectRegistry.TryGet( component, out Guid componentGuid ) )
                {
                    componentGuid = Guid.NewGuid();
                    ObjectRegistry.Register( componentGuid, component );
                }

                JObject componentJson = new JObject()
                {
                    { $"{ObjectSerializer.TYPE}", ObjectSerializer.WriteType( component.GetType() ) },
                    { $"{ObjectSerializer.ID}", ObjectSerializer.WriteGuid(componentGuid) },
                };
                if( component is MeshFilter )
                {
                    componentJson[$"{ObjectSerializer.VALUE}"] = ((MeshFilter)component).WriteJson();
                    componentsJson.Add( componentJson );
                    continue;
                }
                if( component is MeshRenderer )
                {
                    componentJson[$"{ObjectSerializer.VALUE}"] = ((MeshRenderer)component).WriteJson();
                    componentsJson.Add( componentJson );
                    continue;
                }
            }

            JObject json = new JObject()
            {
                { $"{ObjectSerializer.ID}", ObjectSerializer.WriteGuid(guid) },
                { "Name", gameObject.name },
                { "Tag", gameObject.tag },
                { "IsActive", gameObject.activeSelf },
                { "IsStatic", gameObject.isStatic },
                { "Layer", gameObject.layer },
                { "Transform", WriteTransform(gameObject.transform) },
                { "__children", childrenJson },
                { "Components", componentsJson }
            };
            return json;
        }

        public static GameObject ReadGameObject( JObject json, Transform parent = null )
        {
            GameObject go = new GameObject();

            Guid guid = ObjectSerializer.ReadGuid( json[$"{ObjectSerializer.ID}"] );
            ObjectRegistry.Register( guid, go );

            if( parent != null )
            {
                go.transform.SetParent( parent );
            }

            go.name = (string)json["Name"];
            go.tag = (string)json["Tag"];
            go.SetActive( (bool)json["IsActive"] );
            go.isStatic = (bool)json["IsStatic"];
            go.layer = (int)json["Layer"];

            ReadTransform( go, (JObject)json["Transform"] );

            foreach( JObject childJson in json["__children"] )
            {
                GameObject child = ReadGameObject( childJson, go.transform );
            }

            // Store each serializable component and its JSON chunk to deserialize at the end.
            List<(ISelfSerialize iComp, JToken iJson)> componentJsonPair = new List<(ISelfSerialize iComp, JToken iJson)>();

            foreach( JObject componentJson in json["Components"] ) // This doesn't return the UnityEngine.Transform, despite it being derived from UnityEngine.Component.
            {
                Type componentType = ObjectSerializer.ReadType( componentJson[$"{ObjectSerializer.TYPE}"] );

                Guid componentGuid;
                if( typeof( ISelfSerialize ).IsAssignableFrom( componentType ) )
                {
                    ISelfSerialize selfSerializeComponent = ReadCustomPart1( go, componentJson );

                    componentGuid = ObjectSerializer.ReadGuid( componentJson[$"{ObjectSerializer.ID}"] );
                    ObjectRegistry.TryRegister( componentGuid, selfSerializeComponent );

                    componentJsonPair.Add( (selfSerializeComponent, componentJson) );
                    continue;
                }

                Component component = go.AddComponent( componentType );

                componentGuid = ObjectSerializer.ReadGuid( componentJson[$"{ObjectSerializer.ID}"] );
                ObjectRegistry.Register( componentGuid, component );

                if( componentType == typeof( MeshFilter ) )
                {
                    ((MeshFilter)component).ReadJson( (JObject)componentJson[$"{ObjectSerializer.VALUE}"] );
                    continue;
                }
                if( componentType == typeof( MeshRenderer ) )
                {
                    ((MeshRenderer)component).ReadJson( (JObject)componentJson[$"{ObjectSerializer.VALUE}"] );
                    continue;
                }
            }

#warning TODO - this should be called at the very end for the nested gameobjects' components, after the root gameobject has its components added.
            // Calling this after all components have been added allows you to reference a component that will be added after the component that is referencing it.
            // Comp A references Comp B, Comp A is added before Comp B but when the Readjson is called, CompB has been added and should exist in the object registry.
            foreach( var cjp in componentJsonPair )
            {
                cjp.iComp.ReadJson( cjp.iJson[$"{ObjectSerializer.VALUE}"] );
            }

            return go;
        }
    }
}