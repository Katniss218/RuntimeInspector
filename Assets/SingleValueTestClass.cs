using Newtonsoft.Json.Linq;
using RuntimeEditor.UI.Inspector.Attributes;
using RuntimeEditor.Serialization;
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