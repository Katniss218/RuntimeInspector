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
        [field: SerializeField]
        [field: Hide]
        public int[] Value { get; set; }

        [field: Hide]
        public int[][] NestedArrays { get; set; } = new int[][]
        {
            new int[] { 1, 2 },
            new int[] { 2, 3 },
            new int[] { 4, 5 },
        };

        [field: SerializeField]
        [field: Hide]
        public int haha { get; set; }
    }
}