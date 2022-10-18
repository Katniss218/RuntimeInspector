using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuntimeInspector.Core.AssetManagement;
using RuntimeInspector.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttach : MonoBehaviour
{
    [SerializeField] private Material regMat;
    [SerializeField] private GameObject test;

    public class Test
    {
        public Func<int, int, bool> TestField;

        public bool Equal( int a, int b )
        {
            return a == b;
        }
    }

    void Start()
    {/*
        Test donor = new Test();
        donor.TestField = donor.Equal;

        Test obj = new Test();
        obj.TestField = ( a, b ) => false;

        ObjectRegistry.Clear();

        JToken val = ObjectSerializer.WriteDelegate( donor.TestField );

        string json = JsonConvert.SerializeObject( val );

        obj.TestField = (Func<int, int, bool>)ObjectSerializer.ReadDelegate( val );*/

        Mesh mesh = new Mesh();
        AssetRegistry<Mesh>.Register( "Mesh|test_mesh", mesh );
        AssetRegistry<Material>.Register( "Material|default", regMat );

        MeshFilter mf = test.GetComponentInChildren<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = test.GetComponentInChildren<MeshRenderer>();
        mr.sharedMaterial = regMat;

        // Serialize
        ObjectSerializer.StartSerialization();

        JObject val = ComponentSerializer.WriteGameObject( test );

        ObjectSerializer.EndSerialization();

        string json = JsonConvert.SerializeObject( val );

        // Deserialize
        ObjectSerializer.StartSerialization();

        //GameObject newObj = ComponentSerializer.ReadGameObject( val );

        ObjectSerializer.EndSerialization();
    }

    void Update()
    {

    }
}
