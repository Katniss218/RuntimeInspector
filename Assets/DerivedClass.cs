using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedClass : BaseClass
{
    [field: SerializeField]
    public float AdditionalValue { get; set; }

    public override JToken WriteJson()
    {
        JToken json = base.WriteJson();
        json["AdditionalValue"] = this.AdditionalValue;

        return json;
    }

    public override void ReadJson( JToken json )
    {
        base.ReadJson( json );
        this.AdditionalValue = (float)json["AdditionalValue"];
    }
}
