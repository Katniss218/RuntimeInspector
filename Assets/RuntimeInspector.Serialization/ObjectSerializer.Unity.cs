using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeInspector.Serialization
{
    /// <summary>
    /// Populates gameobjects with data from JSON.
    /// </summary>
    public static partial class ObjectSerializer
    {
        public static JToken WriteVector2( Vector2 value )
        {
            return new JArray()
            {
                value.x,
                value.y
            };
        }

        public static Vector2 ReadVector2( JToken json )
        {
            return new Vector2( (float)json[0], (float)json[1] );
        }
        
        public static JToken WriteVector2Int( Vector2Int value )
        {
            return new JArray()
            {
                value.x,
                value.y
            };
        }

        public static Vector2Int ReadVector2Int( JToken json )
        {
            return new Vector2Int( (int)json[0], (int)json[1] );
        }

        public static JToken WriteVector3( Vector3 value )
        {
            return new JArray()
            {
                value.x,
                value.y,
                value.z
            };
        }

        public static Vector3 ReadVector3( JToken json )
        {
            return new Vector3( (float)json[0], (float)json[1], (float)json[2] );
        }

        public static JToken WriteVector3Int( Vector3Int value )
        {
            return new JArray()
            {
                value.x,
                value.y,
                value.z
            };
        }

        public static Vector3Int ReadVector3Int( JToken json )
        {
            return new Vector3Int( (int)json[0], (int)json[1], (int)json[2] );
        }

        public static JToken WriteVector4( Vector4 value )
        {
            return new JArray()
            {
                value.x,
                value.y,
                value.z,
                value.w
            };
        }

        public static Vector4 ReadVector4( JToken json )
        {
            return new Vector4( (float)json[0], (float)json[1], (float)json[2], (float)json[3] );
        }

        public static JToken WriteQuaternion( Quaternion value )
        {
            return new JArray()
            {
                value.x,
                value.y,
                value.z,
                value.w
            };
        }

        public static Quaternion ReadQuaternion( JToken json )
        {
            return new Quaternion( (float)json[0], (float)json[1], (float)json[2], (float)json[3] );
        }
    }
}