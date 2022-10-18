using Newtonsoft.Json.Linq;
using RuntimeInspector.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass : MonoBehaviour, ISelfSerialize
{
    [field: SerializeField]
    public int IntValue { get; set; }

    [field: SerializeField]
    public string StringValue { get; set; }

    [field: SerializeField]
    public MeshFilter Filter { get; set; }

    [field: SerializeField]
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