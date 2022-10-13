using RuntimeInspector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Used to inspect objects.
    /// </summary>
    public class Viewer : MonoBehaviour
    {
        ObjectReflector reflector = new ObjectReflector();

        public Canvas Canvas { get; set; }

        // you'll have the viewer class. that viewer class will set you up when you tell it to.

        // you can then click on stuff to inspect it.
        // clicking on stuff might prove troublesome, not everything will have a collider.

        // need to show hierarchy as well.

        public void SetObject( object obj )
        {
            reflector.BindTo( obj );
        }

        // it can be used to preview gameobjects, as well as other things.
        // the core reflector API should be Unity-agnostic.
        // Unity compat done through extensions.

        public void Show()
        {
            if( !reflector.IsBound )
            {
                return;
            }

            foreach( var member in reflector.AssignableMembers )
            {
                IDrawer drawer = DrawerManager.GetDrawerOfType( member.Type );
                drawer.Draw( member );
            }
        }

        public class Test
        {
            public int integer = 5;
            public string str = "abcd";
        }

        Test test = new Test();

        void Start()
        {
            SetObject( test );
            Show();
        }

        void Update()
        {
            // continuously redraw the values if updated.
        }
    }
}