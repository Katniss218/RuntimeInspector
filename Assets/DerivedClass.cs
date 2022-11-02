using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedClass : BaseClass
{
    [field: SerializeField]
    public float FloatValue { get; set; }

    [field: SerializeField]
    public BaseClass AdditionalValue { get; set; }
    
    [SerializeField]
    private string _readOnly;
    public string ReadOnly { get { return _readOnly; } }

    
    [SerializeField]
    private string _writeOnly;
    public string WriteOnly { set { _writeOnly = value; } }
    
    public override JToken WriteJson()
    {
        JToken json = base.WriteJson();
      //  json["AdditionalValue"] = this.AdditionalValue;

        return json;
    }

    public override void ReadJson( JToken json )
    {
        base.ReadJson( json );
      //  this.AdditionalValue = (float)json["AdditionalValue"];
    }
}
