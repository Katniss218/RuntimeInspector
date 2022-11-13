using Newtonsoft.Json.Linq;
using RuntimeInspector.UI.Inspector.Attributes;
using RuntimeInspector.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class BaseClass : MonoBehaviour, ISelfSerialize
    {
        [field: SerializeField]
        [field: Hide]
        public int IntValue { get; set; }

        [field: SerializeField]
        [field: Hide]
        public string StringValue { get; set; }

        [field: SerializeField]
        [field: Hide]
        public MeshFilter Filter { get; set; }

        [field: SerializeField]
        [field: Hide]
        public MeshRenderer Renderer { get; set; }

        public virtual JToken WriteJson()
        {
            return new JObject()
            {
                { "IntValue", this.IntValue },
                { "StringValue", this.StringValue },
                { "Filter", ObjectSerializer.WriteObjectReference(Filter) },
                { "Renderer", ObjectSerializer.WriteObjectReference(Renderer) }
            };
        }

        public virtual void ReadJson( JToken json )
        {
            this.IntValue = (int)json["IntValue"];
            this.StringValue = (string)json["StringValue"];
            this.Filter = (MeshFilter)ObjectSerializer.ReadObjectReference( json["Filter"] );
            this.Renderer = (MeshRenderer)ObjectSerializer.ReadObjectReference( json["Renderer"] );
        }
    }
}