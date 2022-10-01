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


        GameObject go = new GameObject( "test go" );

        go.transform.position = new Vector3( 5, 3, 2 );

        Mesh mesh = new Mesh();
        AssetManager.Meshes.Register( "Mesh|test_mesh", mesh );

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        JToken val = ComponentSerializer.WriteMeshFilter( mf );

        string json = JsonConvert.SerializeObject( val );
    }

    void Update()
    {

    }
}
