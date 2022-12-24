using Newtonsoft.Json.Linq;
using RuntimeEditor.UI.Inspector.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class DerivedClass : BaseClass
    {
        [field: SerializeField]
        [field: Hide]
        public float FloatValue { get; set; }

        [field: SerializeField]
        [field: Hide]
        [DrawAsValue]
        public BaseClass AdditionalValue { get; set; }

        [field: SerializeField]
        [field: Hide]
        public BaseClass AdditionalValueButNull { get; set; }

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

        public override string ToString()
        {
            return base.ToString();
        }
    }
}