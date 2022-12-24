using Newtonsoft.Json.Linq;
using RuntimeInspector.UI.Inspector.Attributes;
using RuntimeInspector.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SingleValueTestClass : MonoBehaviour
    {
        [field: Hide]
        [DrawAsValue]
        public System.Action Value { get; set; } = () => { };
    }
}